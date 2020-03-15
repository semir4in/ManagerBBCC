using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

using Octokit;

namespace ManagerBBCC.Main.Windows
{
    public partial class MainWindow
    {
        private void SetGithubButton_Click(object sender, RoutedEventArgs e)
        {
            this.GithubFlyout.IsOpen = !this.GithubFlyout.IsOpen;

            if (this.GithubFlyout.IsOpen)
            {
                this.GithubUserNameBox.Text = Core.Setting.GithubUserName;
                this.GithubRepositoryNameBox.Text = Core.Setting.GithubRepositoryName;

                if (Core.Setting.GithubSaved)
                {
                    this.GithubUserNameBox.IsEnabled = false;
                    this.GithubRepositoryNameBox.IsEnabled = false;
                    this.GithubResultBlock.Text = "저장 완료";

                    this.CancelGithubButton.IsEnabled = true;
                    this.TestGithubButton.IsEnabled = false;
                    this.SaveGithubButton.IsEnabled = false;
                    this.VisitGithubButton.IsEnabled = true;
                }
                else
                {
                    this.GithubUserNameBox.IsEnabled = true;
                    this.GithubRepositoryNameBox.IsEnabled = true;
                    this.GithubResultBlock.Text = "연결 안됨";

                    this.CancelGithubButton.IsEnabled = false;
                    this.TestGithubButton.IsEnabled = true;
                    this.SaveGithubButton.IsEnabled = false;
                    this.VisitGithubButton.IsEnabled = false;
                }
            }
        }

        #region [ Flyout -> GithubFlyout ]

        private void CancelGithubButton_Click(object sender, RoutedEventArgs e)
        {
            this.GithubUserNameBox.IsEnabled = true;
            this.GithubRepositoryNameBox.IsEnabled = true;
            this.GithubResultBlock.Text = "저장 취소";

            this.CancelGithubButton.IsEnabled = false;
            this.TestGithubButton.IsEnabled = true;
            this.SaveGithubButton.IsEnabled = false;
            this.VisitGithubButton.IsEnabled = false;

            this.GithubStatusBlock.Text = $"깃허브: 연결 안됨";

            Core.Setting.GithubSaved = false;
        }

        private void TestGithubButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = this.GithubUserNameBox.Text;
            string repositoryName = this.GithubRepositoryNameBox.Text;

            GitHubClient githubClient = null;
            Repository repository = null;
            try
            {
                githubClient = new GitHubClient(new ProductHeaderValue(userName));
                repository = githubClient.Repository.Get(userName, repositoryName).Result;
            }
            catch (Exception)
            {
                Core.BeepSound();
                this.GithubResultBlock.Text = "깃허브 저장소 확인 실패";
                return;
            }

            try
            {
                string[] imagePathSplit = Config.ImageFolderSubPath.Split('\\');
                List<RepositoryContent> imageContents = githubClient.Repository.Content.GetAllContents(userName, repositoryName, imagePathSplit.First()).Result.ToList();
                RepositoryContent dcconContent = imageContents.Find(x => x.Name == imagePathSplit.Last() && x.Type == "dir");
            }
            catch (Exception)
            {
                Core.BeepSound();
                this.GithubResultBlock.Text = "깃허브 이미지 저장경로 확인 실패";
                return;
            }

            try
            {
                List<RepositoryContent> libraryContents = githubClient.Repository.Content.GetAllContents(userName, repositoryName, Config.DataFolderSubPath).Result.ToList();
                RepositoryContent listContent = libraryContents.Find(x => x.Name == Config.JavaScriptFileName && x.Type == "file");
            }
            catch (Exception)
            {
                Core.BeepSound();
                this.GithubResultBlock.Text = "깃허브 데이터 저장경로 확인 실패";
                return;
            }

            this.GithubUserNameBox.IsEnabled = false;
            this.GithubRepositoryNameBox.IsEnabled = false;
            this.GithubResultBlock.Text = "깃허브 저장소 테스트 성공";

            this.CancelGithubButton.IsEnabled = true;
            this.TestGithubButton.IsEnabled = false;
            this.SaveGithubButton.IsEnabled = true;
            this.VisitGithubButton.IsEnabled = true;
        }

        private void SaveGithubButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = this.GithubUserNameBox.Text;
            string repositoryName = this.GithubRepositoryNameBox.Text;

            this.GithubResultBlock.Text = "저장 완료";

            this.SaveGithubButton.IsEnabled = false;
            this.VisitGithubButton.IsEnabled = true;

            this.GithubStatusBlock.Text = $"깃허브: {userName}/{repositoryName}";

            Core.Setting.GithubUserName = userName;
            Core.Setting.GithubRepositoryName = repositoryName;

            Core.Setting.GithubSaved = true;
        }

        private void VisitGithubButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = this.GithubUserNameBox.Text;
            string repositoryName = this.GithubRepositoryNameBox.Text;

            Process.Start($"https://github.com/{userName}/{repositoryName}");
        }

        #endregion
    }
}
