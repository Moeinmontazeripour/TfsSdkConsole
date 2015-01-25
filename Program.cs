using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsSdkConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("please Enter the TFS Server IP : ");
                String TfsServerIp = Console.ReadLine();
                String TfsServerPort = "8080";
                String TfsServerVirtualPath = "tfs";
                String TfsServerUriString = string.Format("http://{0}:{1}/{2}", TfsServerIp, TfsServerPort, TfsServerVirtualPath);
                Uri TfsServerUri = new Uri(TfsServerUriString);

                TfsConfigurationServer TfsServerconfiguration = TfsConfigurationServerFactory.GetConfigurationServer(TfsServerUri);

                ReadOnlyCollection<CatalogNode> ProjectCollectionNodes = TfsServerconfiguration.CatalogNode.QueryChildren(
                 new[] { CatalogResourceTypes.ProjectCollection },
                 false, CatalogQueryOptions.None);


                Console.WriteLine("please enter the ProjectCollection name : ");
                string ProjectCollectionName = Console.ReadLine();

                foreach (CatalogNode ProjectCollectionNode in ProjectCollectionNodes)
                {
                    if (ProjectCollectionNode.Resource.DisplayName == ProjectCollectionName)
                    {
                        Guid collectionId = new Guid(ProjectCollectionNode.Resource.Properties["InstanceId"]);
                        TfsTeamProjectCollection TeamProjectCollection = TfsServerconfiguration.GetTeamProjectCollection(collectionId);
                        TeamProjectCollection.Connect(ConnectOptions.None);
                        VersionControlServer Vcs = TeamProjectCollection.GetService<VersionControlServer>();

                        ReadOnlyCollection<CatalogNode> TeamProjectNodes = ProjectCollectionNode.QueryChildren(
                       new[] { CatalogResourceTypes.TeamProject },
                       false, CatalogQueryOptions.None);

                        Console.WriteLine("please enter the Team Project name : ");
                        String TeamProjectName = Console.ReadLine();

                        Console.WriteLine("Please enter the Branch name : ");
                        String BranchName = Console.ReadLine();

                        foreach (CatalogNode TeamProjectNode in TeamProjectNodes)
                        {

                            if (TeamProjectNode.Resource.DisplayName == TeamProjectName)
                            {



                                var tp = Vcs.GetTeamProject(TeamProjectNode.Resource.DisplayName);
                                var ServerPath = String.Format("{0}/Branches/{1}", tp.ServerItem, BranchName);


                                VersionSpec versionFrom = null;
                                VersionSpec versionTo = VersionSpec.Latest;


                                var ChangeSetList = Vcs.QueryHistory(

                                                         ServerPath,
                                                         VersionSpec.Latest,
                                                         0,
                                                         RecursionType.Full,
                                                         null,
                                                         versionFrom,
                                                         versionTo,
                                                         Int32.MaxValue,
                                                         true,
                                                         true
                                                                ).Cast<Changeset>().ToList();

                                Changeset Lastchangeset = null;
                                Lastchangeset = ChangeSetList.First();

                                Console.WriteLine(".....last Changest Report.....");
                                Console.WriteLine();
                                Console.WriteLine(string.Format("ID : {0} , Date: {1} , Comment : {2} , by : {3} ", Lastchangeset.ChangesetId, Lastchangeset.CreationDate, Lastchangeset.Comment, Lastchangeset.OwnerDisplayName));
                                Console.WriteLine();
                                Console.WriteLine(".....End The Report.....");

                            }

                        }

                    }
                                   }
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(" Warnining ");
                Console.WriteLine("-----------please check your TFS Server IP-----------");
                Console.ReadLine();
            }
        }
    }
}
