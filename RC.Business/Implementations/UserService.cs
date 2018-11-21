using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RC.Business.Interfaces;
using RC.DataAccess.Interfaces;
using RC.Models;
using RC.Models.Account.Request;
using RC.Models.Account.Response;
using RC.Models.Beaer;
using RC.Models.EntityModels;
using RC.Models.Enums;

namespace RC.Business.Implementations
{
    public class UserService : Service<User>, IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IOptions<JwtIssuerSettings> _jwtSettings;
        private readonly IUserTokenRepository _userTokenRepository;

        public UserService(IUserRepositry repositry,
            UserManager<User> userManage,
            RoleManager<Role> roleManager,
            IConfiguration configuration,
            IUserTokenRepository userTokenRepository,
            IOptions<JwtIssuerSettings> jwtSettings,
        SignInManager<User> signInManager) : base(repositry)
        {
            _userManager = userManage;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings;
            _userTokenRepository = userTokenRepository;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                if (user.Status == UserStatus.Inactive)
                {
                    return new LoginResponse
                    {
                        ResponseMessage = new ResponseMessage(ResponseStatus.Status.Fail, @"User is not activated")
                    };
                }

                // update the security stamp only on correct username/password
                await _userManager.UpdateSecurityStampAsync(user);

                // validate password
                var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);

                if (signInResult.Succeeded)
                {
                    // generated token
                    var token = await GenerateJwtTokenAsync(user, request.Remember);
                    if (token != null)
                    {
                        return new LoginResponse
                        {
                            ResponseMessage = new ResponseMessage(ResponseStatus.Status.Success),
                            AccessToken = token.AccessToken,
                            RefreshAccessToken = token.RefreshToken
                        };
                    }
                }
                #region Extend later if need

                //else if (signInResult.IsLockedOut)
                //{
                //    throw new NotImplementedException("RequiresTwoFactor");
                //}
                //else if (signInResult.IsNotAllowed)
                //{
                //}
                //else if (signInResult.RequiresTwoFactor)
                //{
                //    // implement later
                //    throw new NotImplementedException("RequiresTwoFactor");
                //} 
                #endregion
            }

            return new LoginResponse
            {
                ResponseMessage = new ResponseMessage(ResponseStatus.Status.Fail, @"User name or password is invalid")
            };
        }

        public async Task<LoginResponse> RefreshLoginAsync(RefreshLoginRequest request)
        {
            var validateResult = await ValidateRefreshTokenAsync(request.RefreshToken);

            if (validateResult.ResponseMessage.Status != ResponseStatus.Status.Success ||
                validateResult.ClaimsPrincipal == null)
            {
                return new LoginResponse
                {
                    ResponseMessage = new ResponseMessage(ResponseStatus.Status.Fail, @"Token is invalid")
                };
            }

            var userName = validateResult.ClaimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            if (userName == null)
            {
                throw new ArgumentNullException($"Cannot retrive user name");
            }

            var user = await _userManager.FindByNameAsync(userName.Value);
            if (user != null)
            {
                if (user.Status == UserStatus.Inactive)
                {
                    return new LoginResponse
                    {
                        ResponseMessage = new ResponseMessage(ResponseStatus.Status.Fail, @"User is not activated")
                    };
                }

                var token = await GenerateJwtTokenAsync(user, false);
                if (token != null)
                {
                    // for security, each refresh token only use once
                    // delete old refresh token from DB 
                    if (!string.IsNullOrEmpty(validateResult.RefreshTokenKey))
                    {
                        await _userTokenRepository.DeleteAsync(x =>
                            x.UserId == user.Id && x.RefreshTokenKey == validateResult.RefreshTokenKey);
                    }

                    return new LoginResponse
                    {
                        ResponseMessage = new ResponseMessage(ResponseStatus.Status.Success),
                        AccessToken = token.AccessToken,
                        RefreshAccessToken = await GenerateJwtRefreshTokenAsync(user) // new refresh token
                    };
                }
            }

            return new LoginResponse
            {
                ResponseMessage = new ResponseMessage(ResponseStatus.Status.Fail, @"RefreshLoginAsync failed")
            };
        }

        private async Task<JsonWebToken> GenerateJwtTokenAsync(User user, bool remember)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            IdentityOptions options = new IdentityOptions();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Value.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // Claim Type = Name is UserId
                    new Claim(ClaimTypes.Name, user.Id.ToString()),

                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(options.ClaimsIdentity.UserIdClaimType, user.Id.ToString(), ClaimValueTypes.String),
                    new Claim(options.ClaimsIdentity.UserNameClaimType, user.UserName, ClaimValueTypes.String),
                }),

                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.Value.TokenValidFor),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                IssuedAt = DateTime.UtcNow,
                Audience = user.UserName,
                Issuer = _jwtSettings.Value.Issuer
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = string.Empty;

            if (remember)
            {
                refreshToken = await GenerateJwtRefreshTokenAsync(user);
            }


            return new JsonWebToken()
            {
                AccessToken = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken
            };
        }

        private async Task<string> GenerateJwtRefreshTokenAsync(User user)
        {
            var refreshTokenKey = Guid.NewGuid().ToString().Replace("-", "");
            var usertoken = new UserToken()
            {
                UserId = user.Id,
                RefreshTokenKey = refreshTokenKey
            };

            var savetoDb = await _userTokenRepository.AddAsync(usertoken);

            if (savetoDb <= 0)
            {
                throw new Exception("audience");
            }

            var refreshSceretKey = _jwtSettings.Value.RefreshSecretKey; //TODO: get from appSetting.json
            IdentityOptions options = new IdentityOptions();
            var key = Encoding.ASCII.GetBytes(refreshSceretKey);
            var claims = new List<Claim>()
            {
                // Claim Type = Name is UserId
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(options.ClaimsIdentity.UserIdClaimType, user.Id.ToString(), ClaimValueTypes.String),
                new Claim(options.ClaimsIdentity.UserNameClaimType, user.UserName, ClaimValueTypes.String),
                new Claim(JWTInfor.RefreshTokenKey, refreshTokenKey, ClaimValueTypes.String)
        };

            var jwt = new JwtSecurityToken(
                issuer: _jwtSettings.Value.Issuer,
                audience: _jwtSettings.Value.Audience,
                claims: claims,
                notBefore: usertoken.CreationTime,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.Value.RefreshTokenValidFor),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature));

            var refreshToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            return refreshToken;
        }

        private async Task<ValidateRefreshTokenResult> ValidateRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentNullException("refreshToken");
            }

            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = _jwtSettings.Value.Issuer,
                ValidAudiences = new[] { _jwtSettings.Value.Audience },
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(_jwtSettings.Value.RefreshSecretKey))
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            ClaimsPrincipal claimsPrincipal;
            var responseMessage = new ResponseMessage();

            var oldRefreshTokenKey = string.Empty;

            try
            {
                claimsPrincipal = handler.ValidateToken(refreshToken, validationParameters, out var validatedToken);
                // TODO: what is validatedToken, can replace _userTokenRepository.GetObjectByIdAsync(audienceId);?
                var audienceInClaim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == JWTInfor.AudienceId);
                var refreshTokenKeyInClaim =
                    claimsPrincipal.Claims.FirstOrDefault(x => x.Type == JWTInfor.RefreshTokenKey);
                if (audienceInClaim == null || refreshTokenKeyInClaim == null || !Guid.TryParse(audienceInClaim.Value, out var audienceId))
                {
                    return new ValidateRefreshTokenResult
                    {
                        ResponseMessage = new ResponseMessage(ResponseStatus.Status.Invalid, "Audience is not found"),
                        ClaimsPrincipal = null
                    };
                }

                // query db for refresh token
                var audience = await _userTokenRepository.GetObjectByIdAsync(audienceId);
                if (audience == null)
                {
                    return new ValidateRefreshTokenResult
                    {
                        ResponseMessage = new ResponseMessage(ResponseStatus.Status.Invalid, "Audience is not found"),
                        ClaimsPrincipal = null
                    };
                }

                if (!audience.RefreshTokenKey.Equals(refreshTokenKeyInClaim.Value))
                {
                    return new ValidateRefreshTokenResult
                    {
                        ResponseMessage = new ResponseMessage(ResponseStatus.Status.Invalid, "Audience is not found"),
                        ClaimsPrincipal = null
                    };
                }

                if (DateTime.Compare(audience.ExpirationTime, DateTime.UtcNow) < 0)
                {
                    return new ValidateRefreshTokenResult
                    {
                        ResponseMessage = new ResponseMessage(ResponseStatus.Status.Invalid, "Token is expired"),
                        ClaimsPrincipal = null
                    };
                }

                responseMessage.Status = ResponseStatus.Status.Success;
                oldRefreshTokenKey = refreshTokenKeyInClaim.Value;
            }
            catch (SecurityTokenValidationException ex)
            {
                responseMessage.Status = ResponseStatus.Status.Invalid;
                responseMessage.Message = ex.Message;
                claimsPrincipal = null;
            }

            return new ValidateRefreshTokenResult
            {
                ResponseMessage = responseMessage,
                ClaimsPrincipal = claimsPrincipal,
                RefreshTokenKey = oldRefreshTokenKey
            };
        }
    }

}
