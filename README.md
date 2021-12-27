# spgraph_hc
sharepoint folders/subsite graphql explorer using hotchocolate

Use the SharePoint jwt in the Auth header as a bearer token and below is a sample request that can be used to show as a 


Sample request. 

{
  spSite(url: "<subsite url>") {
  siteId,
   spSubsites{
     siteId,
     siteUrl,
     siteTitle
   } ,
   spFolders{
     folders{
       folderTitle
     },
     folderId,
     folderTitle
   }
  }
}

Sample Response : 
"data": {
    "spSite": {
      "siteId" : "GraphQL explorer"
      "spSubsites": [
        ]
      "spFolders": [
        ]
  }
}
