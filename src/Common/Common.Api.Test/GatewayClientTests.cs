﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.PowerBI.Common.Api.Gateways.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Microsoft.PowerBI.Common.Api.Test
{
    [TestClass]
    public class GatewayClientTests
    {
        [TestMethod]
        public async Task AddOnPremisesDataGatewayClusterUserCanBeSerialized()
        {
            // Arrange
            var client = Utilities.GetTestClient("");
            var request = new GatewayClusterAddPrincipalRequest()
            {
                PrincipalObjectId = new Guid(),
                AllowedDataSourceTypes = new DatasourceType[]
                {
                    DatasourceType.Sql
                },
                Role = "the role"

            };

            // Act
            var result = await client.AddUsersToGatewayCluster(new Guid(), request, true);

            // Assert
            result.IsSuccessStatusCode.Should().Be(true);
        }

        [TestMethod]
        public async Task GetOnPremisesDataGatewayClusterStatusJsonResponseCanBeDeSerialized()
        {
            // Arrange
            var clusterStatus = "the cluster status";
            var serializedODataRepsonse = $@"{{
  ""@odata.context"":""http://example.net/v2.0/myorg/me/$metadata#gatewayClusters/status/$entity"",
  ""clusterStatus"":""{clusterStatus}"",
  ""gatewayStaticCapabilities"":""the static capabilities"",
  ""gatewayVersion"":""3000.0.0.0+gabcdef0"",
  ""gatewayUpgradeState"":""the upgrade state""
}}";

            var client = Utilities.GetTestClient(serializedODataRepsonse);

            // Act
            var result = await client.GetGatewayClusterStatus(new Guid(), true);

            // Assert
            result.ClusterStatus.Should().Be(clusterStatus);
        }

        [TestMethod]
        public async Task GetOnPremisesDataGatewayClusterStatusCanBeSerializedAndDeSerialized()
        {
            // Arrange
            var clusterStatus = "the cluster status";
            var oDataResponse = new ODataResponseGatewayClusterStatusResponse
            {
                ODataContext = "http://example.net/v2.0/myorg/me/$metadata#gatewayClusters/status/$entity",
                ClusterStatus = clusterStatus,
                GatewayStaticCapabilities = "the static capabilities",
                GatewayVersion = "3000.0.0.0+gabcdef0",
                GatewayUpgradeState = "the upgrade state"
            };

            var serializedODataRepsonse = JsonConvert.SerializeObject(oDataResponse);
            var client = Utilities.GetTestClient(serializedODataRepsonse);

            // Act
            var result = await client.GetGatewayClusterStatus(new Guid(), true);

            // Assert
            oDataResponse.Should().BeEquivalentTo(result);
        }

        [TestMethod]
        public async Task GetOnPremisesDataGatewayClustersJsonResponseCanBeDeSerialized()
        {
            // Arrange
            var clusterId = new Guid("B3497368-4FC0-409F-A338-827B3B7A6F1C");
            var serializedODataRepsonse = $@"{{
    ""@odata.context"": ""http://example.net/v2.0/myorg/me/$metadata#gatewayClusters"",
    ""value"": [
        {{
            ""id"": ""{clusterId}"",
            ""name"": ""cluster"",
            ""dataSourceIds"": [],
            ""permissions"": [
                {{
                    ""id"": ""490F8A04-ABB8-4015-ABE6-7D361B9135B3"",
                    ""principalType"": ""User"",
                    ""tenantId"": ""7552F012-6C5E-4E5B-8EA2-260215EB8236"",
                    ""role"": ""Admin"",
                    ""allowedDataSources"": [],
                    ""clusterId"": ""{clusterId}""
                }}
            ],
            ""memberGateways"": [
                {{
                    ""id"": ""{clusterId}"",
                    ""name"": ""gateway1"",
                    ""type"": ""Resource"",
                    ""version"": ""3000.0.0"",
                    ""clusterId"": ""{clusterId}""
                }}
            ]
        }}
    ]
}}";
            var client = Utilities.GetTestClient(serializedODataRepsonse);

            // Act
            var result = await client.GetGatewayClusters(true);
            var resultCluster = result.ToArray()[0];

            // Assert
            // Assumption: we leave it to other tests to make sure all values are correct
            // The main purpose of this test is to make sure de-serialization works
            resultCluster.Id.Should().Be(clusterId);
        }

        [TestMethod]
        public async Task GetOnPremisesDataGatewayClustersCanBeSerializedAndDeSerialized()
        {
            // Arrange
            var clusterId = new Guid("B3497368-4FC0-409F-A338-827B3B7A6F1C");
            var gatewayClusterObject = new GatewayCluster
            {
                Id = clusterId,
                Name = "the cluster name",
                Description = "the cluster description",
                Status = "the status",
                Region = "the region",
                Permissions = new Permission[]
                {
                    new Permission
                    {
                        Id = "490F8A04-ABB8-4015-ABE6-7D361B9135B3",
                        PrincipalType = "User",
                        TenantId = "7552F012-6C5E-4E5B-8EA2-260215EB8236",
                        Role = "Admin",
                        AllowedDataSources = new string[]
                        {
                            "EFD4D249-519D-4CE5-BC2D-F7607C30EC02"
                        },
                        ClusterId = clusterId.ToString()
                    }
                },
                DataSourceIds = new Guid[]
                {
                    new Guid("EFD4D249-519D-4CE5-BC2D-F7607C30EC02")
                },
                Type = "the type",
                MemberGateways = new MemberGateway[]
                {
                    new MemberGateway
                    {
                        Id = clusterId,
                        Name = "the member name",
                        Description = "the member description",
                        Status = "the member status",
                        Region = "the member region",
                        Type = "the member type",
                        Version = "the version",
                        Annotation = "the annotation",
                        ClusterId = clusterId
                    }
                }
            };

            var oDataResponse = new ODataResponseList<GatewayCluster>
            {
                ODataContext = "http://example.net/v2.0/myorg/me/$metadata#gatewayClusters",
                Value = new GatewayCluster[]
                {
                    gatewayClusterObject
                }
            };

            var serializedODataRepsonse = JsonConvert.SerializeObject(oDataResponse);
            var client = Utilities.GetTestClient(serializedODataRepsonse);

            // Act
            var result = await client.GetGatewayClusters(true);

            // Assert
            gatewayClusterObject.Should().BeEquivalentTo(result.ToArray()[0]);
        }

        [TestMethod]
        public async Task GetOnPremisesDataGatewayClustersWithClusterIdJsonResponseCanBeDeSerialized()
        {

            // Arrange
            var clusterId = new Guid("B3497368-4FC0-409F-A338-827B3B7A6F1C");
            var serializedOdataRepsonse = $@"{{
    ""@odata.context"": ""http://example.net/v2.0/myorg/me/$metadata#gatewayClusters/$entity"",
    ""id"": ""{clusterId}"",
    ""name"": ""cluster"",
    ""dataSourceIds"": [],
    ""permissions"": [
        {{
            ""id"": ""490F8A04-ABB8-4015-ABE6-7D361B9135B3"",
            ""principalType"": ""User"",
            ""tenantId"": ""7552F012-6C5E-4E5B-8EA2-260215EB8236"",
            ""role"": ""Admin"",
            ""allowedDataSources"": [],
            ""clusterId"": ""{clusterId}""
        }}
    ],
    ""memberGateways"": [
        {{
            ""id"": ""{clusterId}"",
            ""name"": ""gateway1"",
            ""type"": ""Resource"",
            ""version"": ""3000.0.0"",
            ""clusterId"": ""{clusterId}""
        }}
    ]
}}";
            var client = Utilities.GetTestClient(serializedOdataRepsonse);

            // Act
            var result = await client.GetGatewayClusters(clusterId, true);

            // Assert
            result.Id.Should().Be(clusterId);
        }

        [TestMethod]
        public async Task GetOnPremisesDataGatewayClustersWithClusterIdCanBeSerializedAndDeSerialized()
        {
            // Arrange
            var clusterId = new Guid("B3497368-4FC0-409F-A338-827B3B7A6F1C");
            var oDataResponse = new ODataResponseGatewayCluster
            {
                ODataContext = "http://example.net/v2.0/myorg/me/$metadata#gatewayClusters/$entity",
                Id = clusterId,
                Name = "the cluster name",
                Description = "the cluster description",
                Status = "the status",
                Region = "the region",
                Permissions = new Permission[]
                {
                    new Permission
                    {
                        Id = "490F8A04-ABB8-4015-ABE6-7D361B9135B3",
                        PrincipalType = "User",
                        TenantId = "7552F012-6C5E-4E5B-8EA2-260215EB8236",
                        Role = "Admin",
                        AllowedDataSources = new string[]
                        {
                            "EFD4D249-519D-4CE5-BC2D-F7607C30EC02"
                        },
                        ClusterId = clusterId.ToString()
                    }
                },
                DataSourceIds = new Guid[]
                {
                    new Guid("EFD4D249-519D-4CE5-BC2D-F7607C30EC02")
                },
                Type = "the type",
                MemberGateways = new MemberGateway[]
                {
                    new MemberGateway
                    {
                        Id = clusterId,
                        Name = "the member name",
                        Description = "the member description",
                        Status = "the member status",
                        Region = "the member region",
                        Type = "the member type",
                        Version = "the version",
                        Annotation = "the annotation",
                        ClusterId = clusterId
                    }
                }
            };

            var serializedODataRepsonse = JsonConvert.SerializeObject(oDataResponse);
            var client = Utilities.GetTestClient(serializedODataRepsonse);

            // Act
            var result = await client.GetGatewayClusters(clusterId, true);

            // Assert
            oDataResponse.Should().BeEquivalentTo(result);
        }

        [TestMethod]
        public async Task GetOnPremisesDataGatewayTenantPolicyJsonResponseCanBeDeSerialized()
        {

            // Arrange
            var tenantId = "69BFBD2A-1901-48BA-9CAF-0D9190BEE34A";
            var serializedODataRepsonse = $@"{{
  ""@odata.context"":""http://example.net/v2.0/myorg/gatewayPolicy"",
  ""id"":""{tenantId}"",
  ""policy"":0
}}";

            var client = Utilities.GetTestClient(serializedODataRepsonse);

            // Act
            var result = await client.GetTenantPolicy();

            // Assert
            result.TenantObjectId.Should().Be(tenantId);
        }

        [TestMethod]
        public async Task GetOnPremisesDataGatewayInstallersJsonResponseCanBeDeSerialized()
        {

            // Arrange
            var principalObjectId = "836083DE-E806-44E0-9C0D-53BCFD3800CB";
            var serializedODataRepsonse = $@"[
  {{
    ""id"":""{principalObjectId}"",
    ""type"":""Personal""
  }}
]";

            var client = Utilities.GetTestClient(serializedODataRepsonse);

            // Act
            var result = await client.GetInstallerPrincipals(GatewayType.Personal);

            // Assert
            result.ToArray()[0].PrincipalObjectId.Should().Be(principalObjectId);
        }

        [TestMethod]
        public async Task GetOnPremisesDataGatewayInstallersCanBeSerializedAndDeSerialized()
        {
            // Arrange
            var principalObjectId = "836083DE-E806-44E0-9C0D-53BCFD3800CB";
            var oDataResponse = new InstallerPrincipal[]
            {
                new InstallerPrincipal{
                    PrincipalObjectId = principalObjectId,
                    GatewayType = GatewayType.Personal.ToString()
                }
            };

            var serializedODataRepsonse = JsonConvert.SerializeObject(oDataResponse);
            var client = Utilities.GetTestClient(serializedODataRepsonse);

            // Act
            var result = await client.GetInstallerPrincipals(GatewayType.Personal);

            // Assert
            oDataResponse.Should().BeEquivalentTo(result);
        }

        [TestMethod]
        public async Task GetOnPremisesDataGatewayTenantPolicyCanBeSerializedAndDeSerialized()
        {
            // Arrange
            var tenantId = "69BFBD2A-1901-48BA-9CAF-0D9190BEE34A";
            var oDataResponse = new ODataResponseGatewayTenant
            {
                ODataContext = "http://example.net/v2.0/myorg/gatewayPolicy",
                TenantObjectId = tenantId,
                Policy = TenantPolicy.None
            };

            var serializedODataRepsonse = JsonConvert.SerializeObject(oDataResponse);
            var client = Utilities.GetTestClient(serializedODataRepsonse);

            // Act
            var result = await client.GetTenantPolicy();

            // Assert
            oDataResponse.Should().BeEquivalentTo(result);
        }

        [TestMethod]
        public async Task RemoveOnPremisesDataGatewayClusterMemberCanBeSerialized()
        {
            // Arrange
            var client = Utilities.GetTestClient("");

            // Act
            var result = await client.DeleteGatewayClusterMember(new Guid(), new Guid(), true);

            // Assert
            result.IsSuccessStatusCode.Should().Be(true);
        }

        [TestMethod]
        public async Task RemoveOnPremisesDataGatewayClusterCanBeSerialized()
        {
            // Arrange
            var client = Utilities.GetTestClient("");

            // Act
            var result = await client.DeleteGatewayCluster(new Guid(), true);

            // Assert
            result.IsSuccessStatusCode.Should().Be(true);
        }

        [TestMethod]
        public async Task RemoveOnPremisesDataGatewayClusterUserCanBeSerialized()
        {
            // Arrange
            var client = Utilities.GetTestClient("");

            // Act
            var result = await client.DeleteUserOnGatewayCluster(new Guid(), new Guid(), true);

            // Assert
            result.IsSuccessStatusCode.Should().Be(true);
        }

        [TestMethod]
        public async Task SetOnPremisesDataGatewayClusterCanBeSerialized()
        {
            // Arrange
            var client = Utilities.GetTestClient("");
            var request = new PatchGatewayClusterRequest()
            {
                Name = "name",
                Department = "department",
                Description = "description",
                ContactInformation = "contactInformation",
                AllowCloudDatasourceRefresh = true,
                AllowCustomConnectors = true,
                LoadBalancingSelectorType = "loadBalancingSelectorType"
            };

            // Act
            var result = await client.PatchGatewayCluster(new Guid(), request, true);

            // Assert
            result.IsSuccessStatusCode.Should().Be(true);
        }

        [TestMethod]
        public async Task SetOnPremisesDataGatewayInstallersCanBeSerialized()
        {
            // Arrange
            var client = Utilities.GetTestClient("");
            var request = new UpdateGatewayInstallersRequest
            {
                Ids = new string[]
                {
                    "id"
                },
                Operation = OperationType.None,
                GatewayType = GatewayType.Personal
            };

            // Act
            var result = await client.UpdateInstallerPrincipals(request);

            // Assert
            result.IsSuccessStatusCode.Should().Be(true);
        }

        [TestMethod]
        public async Task SetOnPremisesDataGatewayTenantPolicyCanBeSerialized()
        {
            // Arrange
            var client = Utilities.GetTestClient("");
            var request = new UpdateGatewayPolicyRequest()
            {
                ResourceGatewayInstallPolicy = PolicyType.Restricted,
                PersonalGatewayInstallPolicy = PolicyType.None
            };

            // Act
            var result = await client.UpdateTenantPolicy(request);

            // Assert
            result.IsSuccessStatusCode.Should().Be(true);
        }
    }
}
