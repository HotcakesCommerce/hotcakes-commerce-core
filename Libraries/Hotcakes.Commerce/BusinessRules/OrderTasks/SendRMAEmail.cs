#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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

// TODO: We should probably just delete this class.

//using System.Net.Mail;
//using System.Text.RegularExpressions;
//using System;

//namespace Hotcakes.Commerce.BusinessRules.OrderTasks
//{	
//    public class SendRMAEmail : BusinessRules.OrderTask
//    {

//        public override bool Execute(OrderTaskContext context)
//        {
//            if (context.Inputs["rmaid"] != null) {
//                string rmaId = (string)context.Inputs["rmaid"].Value;
//                if (!string.IsNullOrEmpty(rmaId)) {
//                    Orders.RMA rma = Orders.RMA.FindByBvin(rmaId);
//                    if (rma != null) {
//                        if (rma.Bvin != string.Empty) {
//                            string templateBvin = WebAppSettings.RMANewEmailTemplate;
//                            if (templateBvin == string.Empty) {
//                                templateBvin = "c0cb9492-f4be-4bdb-9ae9-0b14c7bd2cd0";
//                            }
//                            string toEmail = context.CurrentRequest.CurrentStore.Settings.MailServer.EmailForNewOrder;
//                            try {
//                                if (toEmail.Trim().Length > 0) {
//                                    Content.EmailTemplate t = Content.EmailTemplate.FindByBvin(templateBvin);
//                                    System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage();
//                                    m = t.ConvertToMailMessage(t.From, toEmail, rma);
//                                    if (m != null) {
//                                        if (!Regex.IsMatch(m.Body, "\\[\\[.+\\]\\]")) {
//                                            if (!Utilities.MailServices.SendMail(m)) {
//                                                EventLog.LogEvent("New RMA Email", "New RMA Email failed to send.", Metrics.EventLogSeverity.Error);
//                                            }

//                                        }
//                                    }
//                                    else {
//                                        EventLog.LogEvent("New RMA Email", "Message was not created successfully.", Metrics.EventLogSeverity.Error);
//                                    }
//                                }
//                            }
//                            catch (Exception ex) {
//                                EventLog.LogEvent(ex);
//                            }
//                        }
//                    }
//                }
//            }

//            return true;
//        }

//        public override bool Rollback(OrderTaskContext context)
//        {
//            return true;
//        }

//        public override string TaskId()
//        {
//            return "e5a9b457-554e-4d31-9c0d-d01d5e0799a3";
//        }

//        public override string TaskName()
//        {
//            return "Send RMA Email";
//        }

//        public override string StepName()
//        {
//            string result = string.Empty;
//            if (result == string.Empty) {
//                result = this.TaskName();
//            }
//            return result;
//        }

//        public override Task Clone()
//        {
//            return new SendRMAEmail();
//        }

//    }
//}

