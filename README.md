# spgraph_hc
sharepoint folders/subsite graphql explorer using hotchocolate

Use the SharePoint jwt in the Auth header as a bearer token and below is a sample request that can be used : 


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

Mutation : 
1. Folder Mutation : create folders in sharepoint folders using mutation. Sample request : 
mutation{
 addFolder(siteUrl:"https://test.sharepoint.com", parent:"<parentfolder>", folderName:"<foldertoCreate>"){
   <folderProperties>
 }
}

Subscription : 
1. Folder subscription : listen to folder creation using websockets. Sample request : 
subscription{
  OnFolderAddition(webUrl:"siteurl"){
    <folderProperties>
  }
}


Feel free to ask/contribute for any change that seem desirable to you.
