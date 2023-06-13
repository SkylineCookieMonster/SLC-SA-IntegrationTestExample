/*
****************************************************************************
*  Copyright (c) 2023,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

12/06/2023	1.0.0.1		MSA, Skyline	Initial version
****************************************************************************
*/

namespace RegressionTests_1
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using QAPortalAPI.Enums;
	using QAPortalAPI.Models.ReportingModels;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			TestReport testReport = new TestReport(
			new TestInfo("DpnLibraryValidaton", "Phoenix", new List<int> { 15337 }, "This test will validate the Core.DataMinerSystem library."),
			new TestSystemInfo("DPN-NPRD-DM01"));

			Stopwatch sw = new Stopwatch();
			IDms thisDms = engine.GetDms();

			sw.Start();
			var elements = thisDms.GetElements();
			sw.Stop();

			double elementRetrieval = sw.Elapsed.TotalMilliseconds;

			sw.Restart();
			var scripts = thisDms.GetScripts();
			sw.Stop();

			double scriptRetrieval = sw.Elapsed.TotalMilliseconds;

			// For testing create failed case */
			testReport.TryAddTestCase(TestCaseReport.GetFailTestCase($"Retrieving elements", "No elements found."));

			if (!scripts.Any())
			{
				string reason = $"No scripts found.";
				testReport.TryAddTestCase(TestCaseReport.GetFailTestCase($"Retrieving scripts", reason));
			}
			else
			{
				testReport.TryAddTestCase(TestCaseReport.GetSuccessTestCase($"Retrieving scripts"));
			}

			testReport.PerformanceTestCases.Add(
				new PerformanceTestCaseReport($"Retrieving elements timing", elementRetrieval < 30000d ? Result.Success : Result.Failure, $"Gives the time needed to get the elements in ms", ResultUnit.Millisecond, elementRetrieval));

			testReport.PerformanceTestCases.Add(
				new PerformanceTestCaseReport($"Retrieving scripts timing", scriptRetrieval < 30000d ? Result.Success : Result.Failure, $"Gives the time needed to get the scripts in ms", ResultUnit.Millisecond, scriptRetrieval));

			engine.AddScriptOutput("report", testReport.ToJson());
		}
	}
}