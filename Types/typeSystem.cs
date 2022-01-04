using System.Collections.Generic;
using GraphQuery;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Mutations;
using Subscriptions;

namespace TypeSystem
{

    public class SPSite
    {
        public string siteId { get; set; }
        public string siteUrl { get; set; }
        public string siteTitle { get; set; }
        public string metadata { get; set; }
        //public List<SPSite> subSites {get;set;}     
    }

    public class SPFolder
    {
        public string folderId { get; set; }
        public string folderTitle { get; set; }

        public string metadata { get; set; }
        public int itemCount { get; set; }
        public string restUrl { get; set; }

    }

    public class SPFile
    {
        public string fileId { get; set; }
        public string fileTitle { get; set; }
        public string metadata { get; set; }

    }

    public class GraphTypes
    {
        public class GraphSpSite : ObjectType<SPSite>
        {
            public Microsoft.AspNetCore.Http.IHttpContextAccessor accessor;

            public GraphSpSite(Microsoft.AspNetCore.Http.IHttpContextAccessor ctx) => this.accessor = ctx;
            protected override void Configure(IObjectTypeDescriptor<SPSite> descriptor)
            {
                var query = new Query(accessor);

                descriptor.Field("spSubsites")
               .Resolve(context =>
               {
                   var sites = query.GetSubSites(context.Parent<SPSite>().siteUrl);
                   return sites;
               });

                descriptor.Field("spFolders")
             .Resolve(context =>
             {
                 var folders = query.GetSiteFolders(context.Parent<SPSite>().siteUrl);
                 return folders;
             }).Type<ListType<GraphSpFolder>>();
            }
        }

        public class GraphSpFolder : ObjectType<SPFolder>
        {
            public Microsoft.AspNetCore.Http.IHttpContextAccessor accessor;

            public GraphSpFolder(Microsoft.AspNetCore.Http.IHttpContextAccessor ctx) => this.accessor = ctx;
            protected override void Configure(IObjectTypeDescriptor<SPFolder> descriptor)
            {
                var query = new Query(accessor);

                descriptor.Field("files")
                .Resolve(context =>
                {
                    var files = query.GetFolderFiles(context.Parent<SPFolder>().restUrl);
                    return files;
                });

                descriptor.Field("folders")
                .Resolve(context =>
                {
                    var folders = query.GetFolders(context.Parent<SPFolder>().restUrl);
                    return folders;
                });
            }
        }


        public class GraphFolderMutation : ObjectType<SPMutationFolder>
        {
            public Microsoft.AspNetCore.Http.IHttpContextAccessor accessor;
            public ITopicEventSender topicSender;
            public GraphFolderMutation(Microsoft.AspNetCore.Http.IHttpContextAccessor ctx, ITopicEventSender sender) { this.accessor = ctx; topicSender = sender; }
            protected override void Configure(IObjectTypeDescriptor<SPMutationFolder> descriptor)
            {
                descriptor.Field("addFolder")
                .Argument("siteUrl", context => context.Type<StringType>())
                .Argument("parent", context => context.Type<StringType>())
                .Argument("folderName", context => context.Type<StringType>())
                .Resolve(context =>
                {
                    var webUrl = context.ArgumentValue<string>("siteUrl");
                    var parentServerRelativeUrl = context.ArgumentValue<string>("parent");
                    var folderName = context.ArgumentValue<string>("folderName");
                    var folderMutation = new SPMutationFolder(accessor);
                    var result = folderMutation.AddFolder(webUrl, parentServerRelativeUrl, folderName).Result;
                    if (result != null)
                        topicSender.SendAsync($"FolderCreated_{webUrl}", result);
                    return result;
                });
            }
        }

        public class GraphFolderSubscription : ObjectType//<Subscriptions.SPFolderSubscription>
        {
            protected override void Configure(IObjectTypeDescriptor descriptor)//<SPFolderSubscription> descriptor)
            {
               descriptor.Field("OnFolderAddition")            
               .Argument("webUrl",w=>w.Type<StringType>()) 
               .Resolve(t=>t.GetEventMessage<SPFolder>())                        
               .Subscribe(async context =>
                {
                    var receiver = context.Service<ITopicEventReceiver>();
                    var webUrl = context.ArgumentValue<string>("webUrl");
                    ISourceStream stream = await receiver.SubscribeAsync<string,SPFolder>($"FolderCreated_{webUrl}");
                    return stream;
                });               
            }
        }

    }


}