SELECT AspNetUsers.UserName, AspNetRoles.Name FROM AspNetUsers, AspNetUserRoles,AspNetRoles
where AspNetUsers.Id = AspNetUserRoles.UserId
and AspNetRoles.id = AspNetUserRoles.RoleId