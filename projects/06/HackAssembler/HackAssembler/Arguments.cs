﻿using System;
using System.IO;

namespace HackAssembler
{
    public class Arguments
    {
        private readonly string _path;

        public string Path => _path;

        public Arguments(string[] args)
        {
            if (args.Length == 0)
                throw new ArgumentException("Path no specified");
            
            if(File.Exists(args[0]) == false)
                throw new ArgumentException($"Cant find file at path {args[0]}");

            _path = args[0];
        }
    }
}