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
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{
    partial class UserQuestionEdit : BaseAdminPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                Page.Form.DefaultButton = CancelImageButton.UniqueID;
                if (Request.QueryString["id"] != null)
                {
                    var question = HccApp.MembershipServices.UserQuestions.Find(Request.QueryString["id"]);
                    ViewState["Question"] = question;
                    if (question.Type == UserQuestionType.MultipleChoice)
                    {
                        QuestionTextBox.Text = question.Values[question.Values.Count - 1].Value;
                        question.Values.RemoveAt(question.Values.Count - 1);
                    }
                }
                else
                {
                    ViewState["Question"] = new UserQuestion();
                }

                InitializeInput();
            }
            else
            {
                var question = (UserQuestion) ViewState["Question"];
                if (QuestionTypeRadioButtonList.SelectedIndex == (int) UserQuestionType.MultipleChoice)
                {
                    foreach (GridViewRow row in ValuesGridView.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            question.Values[row.RowIndex].Value = ((TextBox) row.FindControl("ValueTextBox")).Text;
                        }
                    }
                }
                ViewState["Question"] = question;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (QuestionTypeRadioButtonList.SelectedIndex == (int) UserQuestionType.MultipleChoice)
            {
                var question = (UserQuestion) ViewState["Question"];
                InitializeGrid(question);
            }
        }

        protected void InitializeGrid(UserQuestion question)
        {
            foreach (GridViewRow row in ValuesGridView.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    ((TextBox) row.FindControl("ValueTextBox")).Text = question.Values[row.RowIndex].Value;
                }
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "User Signup Config";
            CurrentTab = AdminTabType.People;
            ValidateCurrentUserHasPermission(SystemPermissions.PeopleView);
        }

        protected void InitializeInput()
        {
            var question = (UserQuestion) ViewState["Question"];
            QuestionTypeRadioButtonList.SelectedIndex = (int) question.Type;
            NameTextBox.Text = question.Name;
            if (question.Type == UserQuestionType.FreeAnswer)
            {
                MultipleChoicePanel.Visible = false;
                if (question.Values.Count == 0)
                {
                    var questionOption = new UserQuestionOption {Bvin = Guid.NewGuid().ToString()};
                    question.Values.Add(questionOption);
                }
                QuestionTextBox.Text = question.Values[0].Value;
            }
            else if (question.Type == UserQuestionType.MultipleChoice)
            {
                MultipleChoicePanel.Visible = true;
                QuestionTypeRadioButtonList.SelectedIndex = (int) question.Type;
                BindQuestionOptionsGrid(question);
            }
        }

        protected void QuestionTypeRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var question = (UserQuestion) ViewState["Question"];
            question.Values.Clear();
            if (QuestionTypeRadioButtonList.SelectedIndex == (int) UserQuestionType.FreeAnswer)
            {
                MultipleChoicePanel.Visible = false;
            }
            else if (QuestionTypeRadioButtonList.SelectedIndex == (int) UserQuestionType.MultipleChoice)
            {
                MultipleChoicePanel.Visible = true;
            }
            ViewState["Question"] = question;
        }

        protected void NewOptionImageButton_Click(object sender, ImageClickEventArgs e)
        {
            var question = (UserQuestion) ViewState["Question"];
            var questionOption = new UserQuestionOption {Bvin = Guid.NewGuid().ToString()};
            question.Values.Add(questionOption);
            BindQuestionOptionsGrid(question);
            ViewState["Question"] = question;
        }

        protected void BindQuestionOptionsGrid(UserQuestion question)
        {
            ValuesGridView.DataSource = question.Values;
            ValuesGridView.DataKeyNames = new[] {"bvin"};
            ValuesGridView.DataBind();
        }

        protected void SaveImageButton_Click(object sender, ImageClickEventArgs e)
        {
            var question = (UserQuestion) ViewState["Question"];
            question.Type = (UserQuestionType) QuestionTypeRadioButtonList.SelectedIndex;
            question.Name = NameTextBox.Text;
            if (question.Type == UserQuestionType.FreeAnswer)
            {
                question.Values.Clear();
                var item = new UserQuestionOption(QuestionTextBox.Text);
                question.Values.Add(item);
            }
            else if (question.Type == UserQuestionType.MultipleChoice)
            {
                foreach (GridViewRow row in ValuesGridView.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        question.Values[row.RowIndex].Value = ((TextBox) row.FindControl("ValueTextBox")).Text;
                    }
                }
                question.Values.Add(new UserQuestionOption(QuestionTextBox.Text));
            }

            if (question.Bvin == string.Empty)
            {
                HccApp.MembershipServices.UserQuestions.Create(question);
            }
            else
            {
                HccApp.MembershipServices.UserQuestions.Update(question);
            }
            Response.Redirect("~/DesktopModules/Hotcakes/Core/Admin/People/UserSignupConfig.aspx");
        }

        protected void ValuesGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var question = (UserQuestion) ViewState["Question"];
            UserQuestionOption itemToRemove = null;
            var key = (string) ValuesGridView.DataKeys[e.RowIndex].Value;
            foreach (var item in question.Values)
            {
                if (item.Bvin == key)
                {
                    itemToRemove = item;
                }
            }
            question.Values.Remove(itemToRemove);
            BindQuestionOptionsGrid(question);
            ViewState["Question"] = question;
        }

        protected void CancelImageButton_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/DesktopModules/Hotcakes/Core/Admin/People/UserSignupConfig.aspx");
        }
    }
}