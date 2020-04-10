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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{
    partial class UserSignupConfig : BaseAdminPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                BindQuestionsGrid();

                //update questions with an order
                var questions = HccApp.MembershipServices.UserQuestions.FindAll();
                var count = 1;
                foreach (var question in questions)
                {
                    if (count == 1)
                    {
                        if (question.Order != 0)
                        {
                            break;
                        }
                    }
                    question.Order = count;
                    count += 1;
                    HccApp.MembershipServices.UserQuestions.Update(question);
                }
            }
        }

        protected void BindQuestionsGrid()
        {
            QuestionsGridView.DataSource = HccApp.MembershipServices.UserQuestions.FindAll();
            QuestionsGridView.DataKeyNames = new[] {"bvin"};
            QuestionsGridView.DataBind();
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "User Signup Config";
            CurrentTab = AdminTabType.People;
            ValidateCurrentUserHasPermission(SystemPermissions.PeopleView);
        }

        protected void QuestionsGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Response.Redirect("~/DesktopModules/Hotcakes/Core/Admin/People/UserQuestionEdit.aspx?id=" +
                              HttpUtility.UrlEncode((string) QuestionsGridView.DataKeys[e.NewEditIndex].Value));
        }

        protected void QuestionsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            HccApp.MembershipServices.UserQuestions.Delete((string) QuestionsGridView.DataKeys[e.RowIndex].Value);
            BindQuestionsGrid();
        }

        protected void NewImageButton_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/DesktopModules/Hotcakes/Core/Admin/People/UserQuestionEdit.aspx");
        }

        protected void QuestionsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "MoveItem")
            {
                if (e.CommandSource is ImageButton)
                {
                    var row = (GridViewRow) ((ImageButton) e.CommandSource).Parent.Parent;
                    var _primaryKey = (string) ((GridView) sender).DataKeys[row.RowIndex].Value;
                    //the down arrow actually moves items up the list
                    if ((string) e.CommandArgument == "Down")
                    {
                        HccApp.MembershipServices.UserQuestions.MoveUp(_primaryKey);
                    }
                    else if ((string) e.CommandArgument == "Up")
                    {
                        HccApp.MembershipServices.UserQuestions.MoveDown(_primaryKey);
                    }
                    BindQuestionsGrid();
                }
            }
        }
    }
}