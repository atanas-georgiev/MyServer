﻿namespace MyServer.Web.Areas.UsersAdmin.Models
{
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;

    using Microsoft.AspNetCore.Mvc;

    using MyServer.Common;
    using MyServer.Data.Models;
    using MyServer.Services.Mappings;
    using MyServer.Web.Areas.Shared.Models;
    using MyServer.Web.Resources;

    public class UsersViewModel : IMapFrom<User>, IHaveCustomMappings
    {
        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(Helpers_SharedResource))
        ]
        [EmailAddress]
        [Display(Name = "Email", ResourceType = typeof(Helpers_SharedResource))]
        [UIHint("KendoTextBox")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(Helpers_SharedResource))
        ]
        [StringLength(50, ErrorMessageResourceName = "ErrorLength",
             ErrorMessageResourceType = typeof(Helpers_SharedResource), MinimumLength = 2)]
        [Display(Name = "FirstName", ResourceType = typeof(Helpers_SharedResource))]
        [UIHint("KendoTextBox")]
        public string FirstName { get; set; }

        [Key]
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(Helpers_SharedResource))
        ]
        [StringLength(50, ErrorMessageResourceName = "ErrorLength",
             ErrorMessageResourceType = typeof(Helpers_SharedResource), MinimumLength = 2)]
        [Display(Name = "LastName", ResourceType = typeof(Helpers_SharedResource))]
        [UIHint("KendoTextBox")]
        public string LastName { get; set; }

        [UIHint("KendoDropDownRoles")]
        [Display(Name = "Role", ResourceType = typeof(Helpers_SharedResource))]
        public MyServerRoles Role { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<User, UsersViewModel>()
                .ForMember(m => m.Role, opt => opt.MapFrom(src => MappingFunctions.MapUserRole(src)));
        }
    }
}