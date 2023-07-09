namespace Pixel.Automation.Agents.Core.Contracts;

/// <summary>
/// Contract to handle and start execution of test cases for a given template
/// Template can have different environment requirements
/// </summary>
public interface ITestExecutionHandler
{   
    /// <summary>
    /// Execute the test case for a given template by starting pixel-run.exe with required parameters
    /// </summary>
    /// <param name="templateName"></param>
    /// <returns></returns>
    Task ExecuteTestAsync(string templateName, Dictionary<string,string> arguments);
}
