using System;
using System.Windows.Forms;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace GuideVkApi
{                                                                                                  //Хочу сделать классы, возможно, но главное - хочу, чтобы имя отправителя отображалось, как имя
                                                                                                      // а не как ID.
    public partial class Form1 : Form
    {
        VkApi vkApi = new VkApi();
        private long userMessageID = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void authorisationButton_Click(object sender, EventArgs e)
        {
            ulong appID = 6651879;
            string email = emailtextBox.Text;
            string password = passTextBox.Text;
            Settings settings = Settings.All;

            ApiAuthParams authParams = new ApiAuthParams()
            {
                ApplicationId = appID,
                Settings = settings,
                Password = password,
                Login = email
            };

            try
            {
                friendsListBox.Items.Clear();
                vkApi.Authorize(authParams);
                GetFrindList();
            }
            catch
            {
                MessageBox.Show(
                    "Something went wrong. Check your password and login. " +
                    "You may also check your internet connection.",
                    "Error");
            }
            
        }

        public void GetFrindList()
        {
            ProfileFields profileFields = ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.Uid;
            var users = vkApi.Friends.Get(new FriendsGetParams { UserId = vkApi.UserId, Fields = profileFields });
            foreach (var friends in users)
            {
                friendsListBox.Items.Add(friends.FirstName + " " + friends.LastName + " | " + friends.Id);
            }
        }

        private void friendsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            userMessageID = long.Parse(friendsListBox.Text.Substring(friendsListBox.Text.LastIndexOf(' ')));
            GetMessageHistory();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (messageTextBox.Text != null)
            {
                vkApi.Messages.Send(new MessagesSendParams
                {
                    UserId = userMessageID,
                    Message = messageTextBox.Text
                });
                messageTextBox.Text = null;
            }
        }

        void GetMessageHistory()
        {
            historyListBox.Items.Clear();
            string currenUserName = vkApi.Account.GetProfileInfoAsync().Result.FirstName;
            var getHistory = vkApi.Messages.GetHistory(new MessagesGetHistoryParams
            {
                Count = 15,
                UserId = long.Parse(userMessageID.ToString())
            });

            foreach (var message in getHistory.Messages )
            {
                if (message.FromId == userMessageID)
                {
                    historyListBox.Items.Add(message.Date + " " +  friendsListBox.Text.Substring(0, friendsListBox.Text.IndexOf(' ')) +
                                             ": " + message.Text + Environment.NewLine);
                }
                else
                {
                    historyListBox.Items.Add(currenUserName + ": " + message.Text);
                }
                
            }
        }
    }
}
