﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Campy.CIL;
using Campy.LCFG;
using Swigged.LLVM;

namespace ConsoleApp4
{
    class Program
    {
        static int Foo1()
        {
            return 1;
        }

        static int Foo2(int a)
        {
            return a + 1;
        }

        static int Foo3(int b)
        {
            if (b > 10)
                return b + 1;
            else
                return b + 2;
            return 99;
        }

        static int fact(int b)
        {
            if (b == 0) return 1;
            else if (b == 1) return 1;
            else return b * fact(b - 1);
        }

        static int SumOf3Or5(int high)
        {
            int result = 0;
            for (int i = 1; i < high; ++i)
            {
                if (i % 3 == 0) result += i;
                if (i % 5 == 0) result += i;
            }
            return result;
        }


        public delegate int DFoo2(int a);

        static void Main(string[] args)
        {
            Reader r = new Reader();
            var mg = r.Cfg;
            mg.StartChangeSet(1);
            r.AnalyzeMethod(() => Program.Foo2(1));
            List<CIL_CFG.Vertex> change_set = mg.EndChangeSet(1);
            var lg = new LLVMCFG();
            var c2 = new Campy.LCFG.Converter(mg, lg);
            Swigged.LLVM.Helper.Adjust.Path();
            c2.ConvertToLLVM(change_set);

            IntPtr p2 = c2.GetPtr(1);
            DFoo2 ff2 = (DFoo2)Marshal.GetDelegateForFunctionPointer(p2, typeof(DFoo2));
            for (int k = 0; k < 100; ++k)
            {
                int result = ff2(k);
                Console.WriteLine("Result is: " + result);
            }

            mg.StartChangeSet(2);
            r.AnalyzeMethod(() => Program.Foo3(2));
            List<CIL_CFG.Vertex> change_set2 = mg.EndChangeSet(2);
            c2.ConvertToLLVM(change_set2);
            IntPtr p3 = c2.GetPtr(3);
            DFoo2 ff3 = (DFoo2)Marshal.GetDelegateForFunctionPointer(p3, typeof(DFoo2));
            for (int k = 0; k < 100; ++k)
            {
                int result = ff3(k);
                Console.WriteLine("Result is: " + result);
            }

            mg.StartChangeSet(3);
            r.AnalyzeMethod(() => Program.fact(2));
            List<CIL_CFG.Vertex> change_set3 = mg.EndChangeSet(3);
            c2.ConvertToLLVM(change_set3);
            IntPtr p4 = c2.GetPtr(7);
            DFoo2 ff4 = (DFoo2)Marshal.GetDelegateForFunctionPointer(p4, typeof(DFoo2));
            for (int k = 0; k < 10; ++k)
            {
                int result = ff4(k);
                Console.WriteLine("Result is: " + result);
            }

            mg.StartChangeSet(4);
            r.AnalyzeMethod(() => Program.SumOf3Or5(2));
            List<CIL_CFG.Vertex> change_set4 = mg.EndChangeSet(4);
            c2.ConvertToLLVM(change_set4);
            IntPtr p5 = c2.GetPtr(13);
            DFoo2 ff5 = (DFoo2)Marshal.GetDelegateForFunctionPointer(p5, typeof(DFoo2));
            for (int k = 0; k < 10; ++k)
            {
                int result = ff5(1000);
                Console.WriteLine("Result is: " + result);
            }

            int pp = SumOf3Or5(1000);
        }
    }
}
