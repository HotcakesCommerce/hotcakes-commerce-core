#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

using System;
using Hotcakes.CommerceDTO.v1.Client;

namespace ApiSample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("This is an API Sample Program for Hotcakes");
            Console.WriteLine();

            var url = string.Empty;
            var key = string.Empty;

            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    url = args[0];
                    key = args[1];
                }
            }

            if (url == string.Empty) url = "http://localhost.hotcakes";
            if (key == string.Empty) key = "1-8a84900c-d8eb-4bb0-b083-9432787137e2";

            var proxy = new Api(url, key);

            var snaps = proxy.CategoriesFindAll();
            if (snaps.Content != null)
            {
                Console.WriteLine("Found " + snaps.Content.Count + " categories");
                Console.WriteLine("-- First 5 --");
                for (var i = 0; i < 5; i++)
                {
                    if (i < snaps.Content.Count)
                    {
                        Console.WriteLine(i + ") " + snaps.Content[i].Name + " [" + snaps.Content[i].Bvin + "]");
                        var cat = proxy.CategoriesFind(snaps.Content[i].Bvin);
                        if (cat.Errors.Count > 0)
                        {
                            foreach (var err in cat.Errors)
                            {
                                Console.WriteLine("ERROR: " + err.Code + " " + err.Description);
                            }
                        }
                        else
                        {
                            Console.WriteLine("By Bvin: " + cat.Content.Name + " | " + cat.Content.Description);
                        }

                        var catSlug = proxy.CategoriesFindBySlug(snaps.Content[i].RewriteUrl);
                        if (catSlug.Errors.Count > 0)
                        {
                            foreach (var err in catSlug.Errors)
                            {
                                Console.WriteLine("ERROR: " + err.Code + " " + err.Description);
                            }
                        }
                        else
                        {
                            Console.WriteLine("By Slug: " + cat.Content.Name + " | " + cat.Content.Description);
                        }
                    }
                }
            }

            Console.WriteLine("Done - Press a key to continue");
            Console.ReadKey();
        }
    }
}