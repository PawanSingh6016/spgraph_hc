using System.Collections.Generic;
using GraphQuery;
using HotChocolate.Types;

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

    }

}