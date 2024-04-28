// See https://aka.ms/new-console-template for more information

using System;
using System.Text.RegularExpressions;
using LightApi.ConsoleSample;
using LightApi.Infra.Helper;
using Masuit.Tools;

// var path=args[0];

// var param=ProcessMessageHelper.Read<ParamDto>(path);

// Console.WriteLine($"读取到了{param.ToJsonString()}");

// param!.Text = "modify";

// Thread.Sleep(10000);

// ProcessMessageHelper.Write(param,path);

string input = "字符串是JSON integer 111111111111111111111111111111111111 is too large or small for an Int32. Path 'age', line 2, position 45.";

// 定义匹配数字的正则表达式
string pattern = @"\b\d+\b";

// 执行正则匹配
MatchCollection matches = Regex.Matches(input, pattern);

// 遍历匹配项
foreach (Match match in matches)
{
    string number = match.Value;
    Console.WriteLine("提取的数字是: " + number);
}

// 如果未找到匹配项
if (matches.Count == 0)
{
    Console.WriteLine("未找到匹配的数字.");
}