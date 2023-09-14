using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using HackVM.Commands;

namespace HackVM
{
    public class CodeTranslator
    {
        private readonly string _newLine = Environment.NewLine;

        private int _branchCounter = 0;
        private Dictionary<string, int> _returnCounters = new Dictionary<string, int>();

        public string TranslateCommand(VMCommand command)
        {
            if (command.CommandType == CommandType.PUSH)
                return TranslatePush(command);
            if (command.CommandType == CommandType.POP)
                return TranslatePop(command);
            if (command.CommandType == CommandType.ARITHMETIC)
                return TranslateArithmetics(command);
            if (command.CommandType == CommandType.LABEL)
                return TranslateLabel(command);
            if (command.CommandType == CommandType.GOTO)
                return TranslateGoTo(command);
            if (command.CommandType == CommandType.IF)
                return TranslateIf(command);
            if (command.CommandType == CommandType.FUNCTION)
                return TranslateFunction(command);
            if (command.CommandType == CommandType.RETURN)
                return TranslateReturn(command);
            if (command.CommandType == CommandType.CALL)
                return TranslateCall(command);
            throw new InvalidEnumArgumentException($"Cant translate command {command.Command}");
        }

        private string TranslateCall(VMCommand command)
        {
            var funcName = $"{command.Arg1}";
            if (_returnCounters.ContainsKey(funcName) == false)
                _returnCounters[funcName] = 0;
            var markName = $"{funcName}$ret.{_returnCounters[funcName]}";
            _returnCounters[funcName]++;
            var offset = 5 + int.Parse(command.Arg2);
            return $"@{markName}{_newLine}" +
                   $"D=A{_newLine}" +
                   $"@SP{_newLine}" +
                   $"A=M{_newLine}" +
                   $"M=D{_newLine}" +
                   $"@SP{_newLine}" +
                   $"M=M+1{_newLine}" +
                   $"" +
                   $"@LCL{_newLine}" +
                   $"D=M{_newLine}" +
                   $"@SP{_newLine}" +
                   $"A=M{_newLine}" +
                   $"M=D{_newLine}" +
                   $"@SP{_newLine}" +
                   $"M=M+1{_newLine}" +
                   $"" +
                   $"@ARG{_newLine}" +
                   $"D=M{_newLine}" +
                   $"@SP{_newLine}" +
                   $"A=M{_newLine}" +
                   $"M=D{_newLine}" +
                   $"@SP{_newLine}" +
                   $"M=M+1{_newLine}" +
                   $"" +
                   $"@THIS{_newLine}" +
                   $"D=M{_newLine}" +
                   $"@SP{_newLine}" +
                   $"A=M{_newLine}" +
                   $"M=D{_newLine}" +
                   $"@SP{_newLine}" +
                   $"M=M+1{_newLine}" +
                   $"" +
                   $"@THAT{_newLine}" +
                   $"D=M{_newLine}" +
                   $"@SP{_newLine}" +
                   $"A=M{_newLine}" +
                   $"M=D{_newLine}" +
                   $"@SP{_newLine}" +
                   $"M=M+1{_newLine}" +
                   $"" +
                   $"@{offset}{_newLine}" +
                   $"D=A{_newLine}" +
                   $"@SP{_newLine}" +
                   $"A=M-D{_newLine}" +
                   $"D=A{_newLine}" +
                   $"@ARG{_newLine}" +
                   $"M=D{_newLine}" +
                   $"" +
                   $"@SP{_newLine}" +
                   $"D=M{_newLine}" +
                   $"@LCL{_newLine}" +
                   $"M=D{_newLine}" +
                   $"" +
                   $"@{funcName}{_newLine}" +
                   $"0;JMP{_newLine}" +
                   $"({markName}){_newLine}";
        }

        private string TranslateReturn(VMCommand command)
        {
            return $"@LCL{_newLine}" +
                   $"D=M{_newLine}" +
                   $"@R14{_newLine}" +
                   $"M=D{_newLine}" +
                   
                   $"@5{_newLine}" +
                   $"D=A{_newLine}" +
                   $"@LCL{_newLine}" +
                   $"A=M-D{_newLine}" +
                   $"D=M{_newLine}" +
                   $"@R13{_newLine}" +
                   $"M=D{_newLine}" +
                   
                   $"@SP{_newLine}" +
                   $"AM=M-1{_newLine}" +
                   $"D=M{_newLine}" +
                   $"@ARG{_newLine}" +
                   $"A=M{_newLine}" +
                   $"M=D{_newLine}" +
                   
                   $"@ARG{_newLine}" +
                   $"D=M+1{_newLine}" +
                   $"@SP{_newLine}" +
                   $"M=D{_newLine}" +
                   
                   $"@1{_newLine}" +
                   $"D=A{_newLine}" +
                   $"@R14{_newLine}" +
                   $"A=M-D{_newLine}" +
                   $"D=M{_newLine}" +
                   $"@THAT{_newLine}" +
                   $"M=D{_newLine}" +
                   
                   $"@2{_newLine}" +
                   $"D=A{_newLine}" +
                   $"@R14{_newLine}" +
                   $"A=M-D{_newLine}" +
                   $"D=M{_newLine}" +
                   $"@THIS{_newLine}" +
                   $"M=D{_newLine}" +
                   
                   $"@3{_newLine}" +
                   $"D=A{_newLine}" +
                   $"@R14{_newLine}" +
                   $"A=M-D{_newLine}" +
                   $"D=M{_newLine}" +
                   $"@ARG{_newLine}" +
                   $"M=D{_newLine}" +
                   
                   $"@4{_newLine}" +
                   $"D=A{_newLine}" +
                   $"@R14{_newLine}" +
                   $"A=M-D{_newLine}" +
                   $"D=M{_newLine}" +
                   $"@LCL{_newLine}" +
                   $"M=D{_newLine}" +
                   
                   $"@R13{_newLine}" +
                   $"A=M{_newLine}" +
                   $"0;JMP{_newLine}";
        }

        private string TranslateFunction(VMCommand command)
        {
            var result = $"({command.Arg1}){_newLine}";
            for (int i = 0; i < int.Parse(command.Arg2); i++)
                result += CleanLocal(i);
            return result;
        }

        private string CleanLocal(int localIndex) =>
            $"@SP{_newLine}" +
            $"A=M{_newLine}" +
            $"M=0{_newLine}" +
            $"@SP{_newLine}" +
            $"M=M+1{_newLine}";

        private string TranslateGoTo(VMCommand command) =>
            $"@{command.Arg1}{_newLine}" +
            $"0;JMP{_newLine}";

        private string TranslateIf(VMCommand command) =>
            $"@SP{_newLine}" +
            $"AM=M-1{_newLine}" +
            $"D=M{_newLine}" +
            $"@{command.Arg1}{_newLine}" +
            $"D;JNE{_newLine}";

        private string TranslateLabel(VMCommand command) =>
            $"({command.Arg1}){_newLine}";

        private string TranslatePush(VMCommand command)
        {
            if (command.Arg1.Equals("local"))
                return LocalPush(command);
            if (command.Arg1.Equals("this"))
                return ThisPush(command);
            if (command.Arg1.Equals("that"))
                return ThatPush(command);
            if (command.Arg1.Equals("argument"))
                return ArgumentPush(command);
            if (command.Arg1.Equals("temp"))
                return TempPush(command);
            if (command.Arg1.Equals("pointer"))
                return PointerPush(command);
            if (command.Arg1.Equals("constant"))
                return ConstantPush(command);
            if (command.Arg1.Equals("static"))
                return StaticPush(command);
            throw new InvalidEnumArgumentException($"Cant translate command {command}");
        }

        private string TranslatePop(VMCommand command)
        {
            if (command.Arg1.Equals("local"))
                return LocalPop(command);
            if (command.Arg1.Equals("this"))
                return ThisPop(command);
            if (command.Arg1.Equals("that"))
                return ThatPop(command);
            if (command.Arg1.Equals("argument"))
                return ArgumentPop(command);
            if (command.Arg1.Equals("temp"))
                return TempPop(command);
            if (command.Arg1.Equals("static"))
                return StaticPop(command);
            if (command.Arg1.Equals("pointer"))
                return PointerPop(command);
            throw new InvalidEnumArgumentException($"Cant translate command {command}");
        }

        private string TranslateArithmetics(VMCommand command)
        {
            if (command.Command.Equals("add"))
                return TranslateAdd();
            if (command.Command.Equals("sub"))
                return TranslateSub();
            if (command.Command.Equals("neg"))
                return TranslateNeg();
            if (command.Command.Equals("eq"))
                return TranslateEq();
            if (command.Command.Equals("gt"))
                return TranslateGt();
            if (command.Command.Equals("lt"))
                return TranslateLt();
            if (command.Command.Equals("and"))
                return TranslateAnd();
            if (command.Command.Equals("or"))
                return TranslateOr();
            if (command.Command.Equals("not"))
                return TranslateNot();
            throw new InvalidEnumArgumentException($"Cant translate command {command}");
        }

        private string PointerPop(VMCommand command)
        {
            var baseAddress = command.Arg2.Equals("0") ? "@THIS" : "@THAT";
            return $"{baseAddress}{_newLine}" +
                   $"D=A{_newLine}" +
                   $"@R13{_newLine}" +
                   $"M=D{_newLine}" +
                   $"@SP{_newLine}" +
                   $"AM=M-1{_newLine}" +
                   $"D=M{_newLine}" +
                   $"@R13{_newLine}" +
                   $"A=M{_newLine}" +
                   $"M=D{_newLine}";
        }

        private string LocalPop(VMCommand command) =>
            SegmentPop(command, "@LCL");

        private string ThisPop(VMCommand command) =>
            SegmentPop(command, "@THIS");

        private string ThatPop(VMCommand command) =>
            SegmentPop(command, "@THAT");

        private string ArgumentPop(VMCommand command) =>
            SegmentPop(command, "@ARG");

        private string TempPop(VMCommand command) =>
            $"@{command.Arg2}{_newLine}" +
            $"D=A{_newLine}" +
            $"@R5{_newLine}" +
            $"A=A+D{_newLine}" +
            $"D=A{_newLine}" +
            $"@R13{_newLine}" +
            $"M=D{_newLine}" +
            $"@SP{_newLine}" +
            $"M=M-1{_newLine}" +
            $"@SP{_newLine}" +
            $"A=M{_newLine}" +
            $"D=M{_newLine}" +
            $"@R13{_newLine}" +
            $"A=M{_newLine}" +
            $"M=D{_newLine}";

        private string SegmentPop(VMCommand command, string segmentBase) =>
            $"@{command.Arg2}{_newLine}" +
            $"D=A{_newLine}" +
            $"{segmentBase}{_newLine}" +
            $"A=M+D{_newLine}" +
            $"D=A{_newLine}" +
            $"@R13{_newLine}" +
            $"M=D{_newLine}" +
            $"@SP{_newLine}" +
            $"AM=M-1{_newLine}" +
            $"D=M{_newLine}" +
            $"@R13{_newLine}" +
            $"A=M{_newLine}" +
            $"M=D{_newLine}";

        private string StaticPop(VMCommand command) =>
            $"@SP{_newLine}" +
            $"M=M-1{_newLine}" +
            $"@SP{_newLine}" +
            $"A=M{_newLine}" +
            $"D=M{_newLine}" +
            $"@{command.FileName}.{command.Arg2}{_newLine}" +
            $"M=D{_newLine}";

        private string LocalPush(VMCommand command) =>
            PushFromSegment(command, "@LCL");

        private string ArgumentPush(VMCommand command) =>
            PushFromSegment(command, "@ARG");

        private string ThatPush(VMCommand command) =>
            PushFromSegment(command, "@THAT");

        private string ThisPush(VMCommand command) =>
            PushFromSegment(command, "@THIS");

        private string PointerPush(VMCommand command)
        {
            var baseAddress = command.Arg2.Equals("0") ? "@THIS" : "@THAT";
            return $"{baseAddress}{_newLine}" +
                   $"D=M{_newLine}" +
                   WriteToStackEnd();
        }

        private string StaticPush(VMCommand command) =>
            $"@{command.FileName}.{command.Arg2}{_newLine}" +
            $"D=M{_newLine}" +
            WriteToStackEnd();

        private string ConstantPush(VMCommand command) =>
            $"@{command.Arg2}{_newLine}" +
            $"D=A{_newLine}" +
            WriteToStackEnd();

        private string TempPush(VMCommand command) =>
            $"@{command.Arg2}{_newLine}" +
            $"D=A{_newLine}" +
            $"@R5{_newLine}" +
            $"A=A+D{_newLine}" +
            $"D=M{_newLine}" +
            WriteToStackEnd();

        private string PushFromSegment(VMCommand command, string segmentBase) =>
            $"@{command.Arg2}{_newLine}" +
            $"D=A{_newLine}" +
            $"{segmentBase}{_newLine}" +
            $"A=M+D{_newLine}" +
            $"D=M{_newLine}" +
            WriteToStackEnd();

        private string WriteToStackEnd() =>
            $"@SP{_newLine}" +
            $"A=M{_newLine}" +
            $"M=D{_newLine}" +
            $"@SP{_newLine}" +
            $"M=M+1{_newLine}";


        private string TranslateAdd() =>
            TranslateTwoOperand("+");

        private string TranslateSub() =>
            TranslateTwoOperand("-");

        private string TranslateAnd() =>
            TranslateTwoOperand("&");

        private string TranslateOr() =>
            TranslateTwoOperand("|");

        private string TranslateEq() =>
            TranslateComparison("JEQ");

        private string TranslateGt() =>
            TranslateComparison("JLT");

        private string TranslateLt() =>
            TranslateComparison("JGT");

        private string TranslateNot() =>
            TranslateSingleOperand("!");

        private string TranslateNeg() =>
            TranslateSingleOperand("-");

        private string TranslateSingleOperand(string operand) =>
            $"@SP{_newLine}" +
            $"A=M-1{_newLine}" +
            $"M={operand}M{_newLine}";

        private string TranslateTwoOperand(string operand) =>
            $"@SP{_newLine}" +
            $"AM=M-1{_newLine}" +
            $"D=M{_newLine}" +
            $"A=A-1{_newLine}" +
            $"M=M{operand}D{_newLine}";

        private string TranslateComparison(string comparison)
        {
            var branchNumber = _branchCounter++;
            return $"@SP{_newLine}" +
                   $"AM=M-1{_newLine}" +
                   $"D=M{_newLine}" +
                   $"@SP{_newLine}" +
                   $"AM=M-1{_newLine}" +
                   $"D=D-M{_newLine}" +
                   $"@TRUE{branchNumber}{_newLine}" +
                   $"D;{comparison}{_newLine}" +
                   $"@SP{_newLine}" +
                   $"A=M{_newLine}" +
                   $"M=0{_newLine}" +
                   $"@EXIT{branchNumber}{_newLine}" +
                   $"0;JMP{_newLine}" +
                   $"(TRUE{branchNumber}){_newLine}" +
                   $"@SP{_newLine}" +
                   $"A=M{_newLine}" +
                   $"M=-1{_newLine}" +
                   $"(EXIT{branchNumber}){_newLine}" +
                   $"@SP{_newLine}" +
                   $"M=M+1{_newLine}";
        }
    }
}