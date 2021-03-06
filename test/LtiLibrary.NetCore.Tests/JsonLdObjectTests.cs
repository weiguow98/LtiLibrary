﻿using System;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.Lis.v1;
using LtiLibrary.NetCore.Tests.SimpleHelpers;
using Newtonsoft.Json;
using Xunit;

namespace LtiLibrary.NetCore.Tests
{
    public class JsonLdObjectTests
    {
        [Fact]
        public void ExternalAndInternalContexts_MatchesReferenceJson()
        {
            var parent = new Parent
            {
                Id = new Uri("http://localhost/test/1"),
                Name = "MyParent",
                Child = new Child
                {
                    Name = "MyChild",
                    GrandChild = new Child
                    {
                        Name = "MyGrandChild"
                    }
                }
            };
            JsonAssertions.AssertSameJsonLdObject(parent, "ExternalAndInternalContexts");
        }

        [Fact]
        public void ExternalContextOnly_MatchesReferenceJson()
        {
            var parent = new Parent
            {
                Id = new Uri("http://localhost/test/1"),
                Name = "MyParent"
            };
            JsonAssertions.AssertSameJsonLdObject(parent, "ExternalContextOnly");
        }

        [Fact]
        public void ExternalContextOnly_ToJsonLdStringMatchesReferenceJson()
        {
            var parent = new Parent
            {
                Id = new Uri("http://localhost/test/1"),
                Name = "MyParent"
            };
            JsonAssertions.AssertSameJsonLdObject(parent, "ExternalContextOnly");
        }

        [Fact]
        public void InternalContextOnly_MatchesReferenceJson()
        {
            var child = new Child
            {
                Name = "MyChild",
                GrandChild = new Child
                {
                    Name = "MyGrandChild"
                }
            };
            JsonAssertions.AssertSameJsonLdObject(child, "InternalContextOnly");
        }

        [Fact]
        public void ContextRolesAreConvertedToJsonLdValues()
        {
            const ContextRole contextRole = ContextRole.Instructor;
            Assert.Equal("\"lism:Instructor\"", contextRole.ToJsonLdString());
        }

        [Fact]
        private void ContextRolesAreConvertedFromJsonLdObjects()
        {
            var expectedContextRoles = new ContextRole[]
            {
                ContextRole.Instructor,
                ContextRole.Administrator,
                ContextRole.Learner,
                ContextRole.ContentDeveloper
            };

            var inputContextRoles = "{ roles: [ " +
                "\"lism:Instructor\", " +
                "\"http://purl.imsglobal.org/vocab/lis/v2/person#Administrator\", " +
                "\"Learner\", " +
                "\"urn:lti:role:ims/lis/ContentDeveloper/ContentExpert\"" +
            "]}";
            var actualContextRoles = JsonConvert.DeserializeObject<RoleTestObject>(inputContextRoles);
            Assert.Equal(actualContextRoles.Roles, expectedContextRoles);
        }
    }

    public class Parent : JsonLdObject
    {
        public Parent()
        {
            ExternalContextId = new Uri("http://localhost/ParentContext");
            Type = "Parent";
        }

        public Child Child { get; set; }

        public string Name { get; set; }
    }

    public class Child : JsonLdObject
    {
        public Child()
        {
            Terms.Add("Child", "ChildTerms");
            Type = "Child";
        }

        public string Name { get; set; }

        public Child GrandChild { get; set; }
    }

    internal class RoleTestObject : JsonLdObject
    {
        public ContextRole[] Roles { get; set; }
    }
}
