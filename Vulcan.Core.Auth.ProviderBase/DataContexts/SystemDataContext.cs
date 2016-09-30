using System;
using System.Collections.Generic;
using Vulcan.Core.Auth.DataSets;
using Vulcan.Core.DataAccess;

namespace Vulcan.Core.Auth.DataContexts
{
    public class SystemDataContext : DynamicDataContext
    {
        private readonly Dictionary<Type, DataSet> _dataSets;

        public SystemDataContext(string tentatId)
            : base("SharedDataContext", tentatId)
        {
            _dataSets = new Dictionary<Type, DataSet>();
        }

        private AudiencesDataSet _audience;
        public AudiencesDataSet Audiences
        {
            get { return _audience ?? (_audience = new AudiencesDataSet(this)); }
            set { _audience = value; }
        }

        private ApiClientsDataSet _apiClients;
        public ApiClientsDataSet ApiClients
        {
            get { return _apiClients ?? (_apiClients = new ApiClientsDataSet(this)); }
            set { _apiClients = value; }
        }

        private RolesDataSet _roles;
        public RolesDataSet Roles
        {
            get { return _roles ?? (_roles = new RolesDataSet(this)); }
            set { _roles = value; }
        }

        private ApiUsersDataSet _apiUsers;
        public ApiUsersDataSet ApiUsers
        {
            get { return _apiUsers ?? (_apiUsers = new ApiUsersDataSet(this)); }
            set { _apiUsers = value; }
        }

        private RefreshTokensDataSet _refreshTokens;
        public RefreshTokensDataSet RefreshTokens
        {
            get { return _refreshTokens ?? (_refreshTokens = new RefreshTokensDataSet(this)); }
            set { _refreshTokens = value; }
        }

        public OrganizationsDataSet _organizations;
        public OrganizationsDataSet Organizations
        {
            get { return _organizations ?? (_organizations = new OrganizationsDataSet(this)); }
            set { _organizations = value; }
        }

        private GroupsDataSet _groups;
        public GroupsDataSet Groups
        {
            get { return _groups ?? (_groups = new GroupsDataSet(this)); }
            set { _groups = value; }
        }

        private GroupRolesDataSet _groupRoles;
        public GroupRolesDataSet GroupRoles
        {
            get { return _groupRoles ?? (_groupRoles = new GroupRolesDataSet(this)); }
            set { _groupRoles = value; }
        }

        private GroupUsersDataSet _groupUsers;
        public GroupUsersDataSet GroupUsers
        {
            get { return _groupUsers ?? (_groupUsers = new GroupUsersDataSet(this)); }
            set { _groupUsers = value; }
        }

        public InterTimeZonesDataSet _timezones;
        public InterTimeZonesDataSet Timezones
        {
            get { return _timezones ?? (_timezones = new InterTimeZonesDataSet(this)); }
            set { _timezones = value; }
        }
        public InterTimeZoneUTCDataSet _timezoneUTCs;
        public InterTimeZoneUTCDataSet TimezoneUTCs
        {
            get { return _timezoneUTCs ?? (_timezoneUTCs = new InterTimeZoneUTCDataSet(this)); }
            set { _timezoneUTCs = value; }
        }
        public T DataSet<T>() where T : DataSet
        {
            var type = typeof (T);
            if (_dataSets.ContainsKey(type))
                return (T)_dataSets[type];

            var instance = (T)Activator.CreateInstance(typeof(T), this);
            _dataSets.Add(type, instance);
            return instance;
        }
    }
}