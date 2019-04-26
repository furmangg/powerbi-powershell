﻿/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

using System;
using System.Management.Automation;
using Microsoft.PowerBI.Commands.Common.Test;
using Microsoft.PowerBI.Commands.Profile.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.PowerBI.Commands.OnPremisesDataGateway.Test
{
    [TestClass]
    public class RemoveOnPremisesDataGatewayClusterMemberTests
    {
        private static CmdletInfo RemoveOnPremisesDataGatewayClusterMemberInfo { get; } = new CmdletInfo(
            $"{RemoveOnPremisesDataGatewayClusterMember.CmdletVerb}-{RemoveOnPremisesDataGatewayClusterMember.CmdletName}",
            typeof(RemoveOnPremisesDataGatewayClusterMember));

        [TestMethod]
        [TestCategory("Interactive")]
        [TestCategory("SkipWhenLiveUnitTesting")] // Ignore for Live Unit Testing
        public void EndToEndRemoveOnPremisesDataGatewayClusterMember()
        {
            using (var ps = System.Management.Automation.PowerShell.Create())
            {
                // Arrange
                ProfileTestUtilities.ConnectToPowerBI(ps);
                ps.AddCommand(RemoveOnPremisesDataGatewayClusterMemberInfo)
                    .AddParameter(nameof(RemoveOnPremisesDataGatewayClusterMember.GatewayClusterId), new Guid())
                    .AddParameter(nameof(RemoveOnPremisesDataGatewayClusterMember.MemberGatewayId), new Guid("{F31FEA72-8435-4871-BF75-E94168C71A6D}"));

                // Act
                var result = ps.Invoke();

                // Assert
                TestUtilities.AssertNoCmdletErrors(ps);
                Assert.IsNotNull(result);
            }
        }
    }
}
