/*

Post-Deployment Script Template
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.
 Use SQLCMD syntax to include a file in the post-deployment script.
 Example:      :r .\myfile.sql
 Use SQLCMD syntax to reference a variable in the post-deployment script.
 Example:      :setvar TableName MyTable
               SELECT * FROM [$(TableName)]
--------------------------------------------------------------------------------------

*/

:r .\StaticData.sql
:r .\TimeZones.sql
:r .\..\Triggers\ApiClientOriginsAuditTrigger.sql
:r .\..\Triggers\ApiClientRolesAuditTrigger.sql
:r .\..\Triggers\ApiClientsAuditTrigger.sql
:r .\..\Triggers\ApiUserRolesAuditTrigger.sql
:r .\..\Triggers\ApiUsersAuditTrigger.sql
:r .\..\Triggers\AudiencesAuditTrigger.sql
:r .\..\Triggers\GroupRolesAuditTrigger.sql
:r .\..\Triggers\GroupsAuditTrigger.sql
:r .\..\Triggers\GroupUsersAuditTrigger.sql
:r .\..\Triggers\OrganizationsAuditTrigger.sql
:r .\..\Triggers\RolesAuditTrigger.sql