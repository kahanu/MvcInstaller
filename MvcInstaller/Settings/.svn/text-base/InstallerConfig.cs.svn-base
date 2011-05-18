// Copyright (c) 2010, Gizmo Beach.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// - Neither  the  name  of  the  Gizmo Beach  nor   the   names  of  its
//   contributors may be used to endorse or  promote  products  derived  from  this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,  BUT  NOT  LIMITED TO, THE IMPLIED
// WARRANTIES  OF  MERCHANTABILITY   AND  FITNESS  FOR  A  PARTICULAR  PURPOSE  ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL,  SPECIAL,  EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO,  PROCUREMENT  OF  SUBSTITUTE  GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)  HOWEVER  CAUSED AND ON
// ANY  THEORY  OF  LIABILITY,  WHETHER  IN  CONTRACT,  STRICT  LIABILITY,  OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE)  ARISING  IN  ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MvcInstaller.Settings
{
    [Serializable]
    public class User
    {
        public User()
        {

        }

        [XmlAttribute()]
        public string UserName;

        [XmlAttribute()]
        public string Password;

        [XmlAttribute()]
        public string Email;

        [XmlAttribute()]
        public string SecretQuestion;

        [XmlAttribute()]
        public string SecretAnswer;
    }

    [Serializable]
    public class Role
    {
        public Role()
        {

        }

        List<User> _userList = new List<User>();

        public List<User> Users
        {
            get { return _userList; }
        }

        [XmlAttribute()]
        public string Name;
    }

    [Serializable]
    public class RoleManager
    {
        public RoleManager()
        {

        }

        //[XmlAttribute()]
        //public bool Create;

        [XmlAttribute()]
        public string ProviderName;

        private List<Role> _roleList = new List<Role>();

        public List<Role> Roles
        {
            get { return _roleList; }
        }
    }

    [Serializable]
    public class Profile
    {
        public Profile()
        {

        }

        //[XmlAttribute()]
        //public bool Create;

        [XmlAttribute()]
        public string ProviderName;
    }

    [Serializable]
    public class Membership
    {
        public Membership()
        {

        }

        [XmlAttribute()]
        public bool Create;

        [XmlAttribute()]
        public string ProviderName;
    }

    [Serializable]
    public class Database
    {
        public Database()
        {

        }

        [XmlAttribute()]
        public bool UseTrustedConnection;

        [XmlAttribute()]
        public string EntityFrameworkEntitiesName;

        public string ConnectionStringName;
        public string DataSource;
        public string InitialCatalog;
        public string UserName;
        public string Password;
    }

    [Serializable]
    public class Path
    {
        public Path()
        {

        }

        public string AppPath;
        public string RelativeSqlPath;
    }

    [Serializable]
    public class InstallerConfig
    {
        public InstallerConfig()
        {

        }

        public string ApplicationName;

        private Database _database = new Database();
        private Membership _membership = new Membership();
        private RoleManager _roleManager = new RoleManager();
        private Profile _profile = new Profile();

        public RoleManager RoleManager
        {
            get { return _roleManager; }
            set { _roleManager = value; }
        }

        public Profile Profile
        {
            get { return _profile; }
            set { _profile = value; }
        }

        public Membership Membership
        {
            get { return _membership; }
            set { _membership = value; }
        }

        public Database Database
        {
            get { return _database; }
            set { _database = value; }
        }

        public Path Path;
    }
}
