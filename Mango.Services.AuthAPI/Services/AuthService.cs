using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dtos;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Mango.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            // check if user exists with the given emial.
            var user = _context.ApplicationUsers.FirstOrDefault(l => l.Email.ToLower() == email.ToLower());


            if(user != null)
            {
                // user found. Now check if the role exists.
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    // create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                // role is created. now assign the role to the user.
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            // fetch the user from db based on username;
            var user = _context.ApplicationUsers.FirstOrDefault(l => l.UserName.ToLower() == loginRequestDto.UesrName.ToLower());

            // check is the password is valid
            var isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (user == null || isValid == false)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""
                };
            }

            // if user was found, Generate JWT Token
            var token = _jwtTokenGenerator.GenerateToken(user);

            UserDto userDto = new UserDto
            {
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ID = user.Id
            };

            LoginResponseDto loginResponse = new LoginResponseDto()
            {
                User = userDto,
                Token = token
            };

            return loginResponse;


        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = registrationRequestDto.Email, // email will be username. (not name)
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber,
            };

            try
            {
                // things like hasing the password will be handled by .net identity
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _context.ApplicationUsers.First(l => l.UserName == registrationRequestDto.Email);

                    UserDto userDto = new UserDto
                    {
                        Email = userToReturn.Email,
                        Name = userToReturn.Name,
                        ID = userToReturn.Id,
                        PhoneNumber = userToReturn.PhoneNumber
                    };

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch(Exception ex)
            {

            }
            return $"Error Encountered during the registration of user {user.NormalizedEmail}";
        }
    }
}
