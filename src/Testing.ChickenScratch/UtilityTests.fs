namespace Testing.ChickenScratch.Utility

open System
open Xunit
open FsUnit.Xunit
open ChickenScratch.Utility


module StringTests =
    [<Theory>]
    [<InlineData("", "")>]
    [<InlineData("    ", "")>]
    [<InlineData("input", "Input")>]
    [<InlineData("Input", "Input")>]
    [<InlineData("hello world", "HelloWorld")>]
    [<InlineData("   one    two    three   ", "OneTwoThree")>]
    [<InlineData("OneTwoThree", "OneTwoThree")>]
    [<InlineData("twoThreeFour", "TwoThreeFour")>]
    [<InlineData("three_four_five", "ThreeFourFive")>]
    [<InlineData("this-that", "ThisThat")>]
    [<InlineData("one two-three_four.five!six^seven*eight", "OneTwoThreeFourFiveSixSevenEight")>]
    [<InlineData("some 1 say 2 things", "Some1Say2Things")>]
    [<InlineData("new\nline\tand\r\n\ttab", "NewLineAndTab")>]
    [<InlineData("ONE TWO THREE", "OneTwoThree")>]
    [<InlineData("act: acronym capitalization test", "ActAcronymCapitalizationTest")>]
    [<InlineData("ADT: acronym decapitalization test", "AdtAcronymDecapitalizationTest")>]
    [<InlineData("your ACT score", "YourActScore")>]
    [<InlineData("YourACTScore", "YourActscore")>]
    [<InlineData("YourActScore", "YourActScore")>]
    [<InlineData("yourActScore", "YourActScore")>]
    [<InlineData("your_act_score", "YourActScore")>]
    [<InlineData("a b c def", "ABCDef")>]
    let ``Can convert to Pascal Case`` input expected = 
        input |> String.toNamingConvention PascalCase |> should equal expected


    [<Theory>]
    [<InlineData("", "")>]
    [<InlineData("    ", "")>]
    [<InlineData("input", "input")>]
    [<InlineData("Input", "input")>]
    [<InlineData("hello world", "helloWorld")>]
    [<InlineData("   one    two    three   ", "oneTwoThree")>]
    [<InlineData("OneTwoThree", "oneTwoThree")>]
    [<InlineData("twoThreeFour", "twoThreeFour")>]
    [<InlineData("three_four_five", "threeFourFive")>]
    [<InlineData("this-that", "thisThat")>]
    [<InlineData("one two-three_four.five!six^seven*eight", "oneTwoThreeFourFiveSixSevenEight")>]
    [<InlineData("some 1 say 2 things", "some1Say2Things")>]
    [<InlineData("new\nline\tand\r\n\ttab", "newLineAndTab")>]
    [<InlineData("ONE TWO THREE", "oneTwoThree")>]
    [<InlineData("act: acronym capitalization test", "actAcronymCapitalizationTest")>]
    [<InlineData("ADT: acronym decapitalization test", "adtAcronymDecapitalizationTest")>]
    [<InlineData("your ACT score", "yourActScore")>]
    [<InlineData("YourACTScore", "yourActscore")>]
    [<InlineData("YourActScore", "yourActScore")>]
    [<InlineData("yourActScore", "yourActScore")>]
    [<InlineData("your_act_score", "yourActScore")>]
    [<InlineData("a b c def", "aBCDef")>]
    let ``Can convert to Camel Case`` input expected = 
        input |> String.toNamingConvention CamelCase |> should equal expected


    [<Theory>]
    [<InlineData("", "")>]
    [<InlineData("    ", "")>]
    [<InlineData("input", "input")>]
    [<InlineData("Input", "input")>]
    [<InlineData("hello world", "hello_world")>]
    [<InlineData("   one    two    three   ", "one_two_three")>]
    [<InlineData("OneTwoThree", "one_two_three")>]
    [<InlineData("twoThreeFour", "two_three_four")>]
    [<InlineData("three_four_five", "three_four_five")>]
    [<InlineData("this-that", "this_that")>]
    [<InlineData("one two-three_four.five!six^seven*eight", "one_two_three_four_five_six_seven_eight")>]
    [<InlineData("some 1 say 2 things", "some_1_say_2_things")>]
    [<InlineData("new\nline\tand\r\n\ttab", "new_line_and_tab")>]
    [<InlineData("ONE TWO THREE", "one_two_three")>]
    [<InlineData("act: acronym capitalization test", "act_acronym_capitalization_test")>]
    [<InlineData("ADT: acronym decapitalization test", "adt_acronym_decapitalization_test")>]
    [<InlineData("your ACT score", "your_act_score")>]
    [<InlineData("YourACTScore", "your_actscore")>]
    [<InlineData("YourActScore", "your_act_score")>]    
    [<InlineData("yourActScore", "your_act_score")>]    
    [<InlineData("your_act_score", "your_act_score")>]    
    [<InlineData("a b c def", "a_b_c_def")>]
    let ``Can convert to Snake Case`` input expected = 
        input |> String.toNamingConvention SnakeCase |> should equal expected


    [<Theory>]
    [<InlineData("", "")>]
    [<InlineData("    ", "")>]
    [<InlineData("input", "input")>]
    [<InlineData("Input", "Input")>]
    [<InlineData("input1", "input1")>]
    [<InlineData("2input", "``2input``")>]
    [<InlineData("hello world", "``hello world``")>]
    [<InlineData("   untrimmed   ", "``   untrimmed   ``")>]
    [<InlineData("symbol!", "``symbol!``")>]
    [<InlineData("symbol#", "``symbol#``")>]
    [<InlineData("$symbol", "``$symbol``")>]
    [<InlineData("sym^bol", "``sym^bol``")>]
    [<InlineData("_symbol", "_symbol")>]
    [<InlineData("symbol_", "symbol_")>]
    [<InlineData("sym_bol", "sym_bol")>]
    let ``Can convert to BackTicks`` input expected = 
        input |> String.toNamingConvention BackTicks |> should equal expected
    