<a name='assembly'></a>
# Keywords.API.Swagger



## Contents

- [IndexerControllerBase](#T-Keywords-API-Swagger-Controllers-Generated-IndexerControllerBase 'Keywords.API.Swagger.Controllers.Generated.IndexerControllerBase')
  - [GetOcrList(videoId)](#M-Keywords-API-Swagger-Controllers-Generated-IndexerControllerBase-GetOcrList-System-String- 'Keywords.API.Swagger.Controllers.Generated.IndexerControllerBase.GetOcrList(System.String)')
  - [IndexVideo(url,name,description)](#M-Keywords-API-Swagger-Controllers-Generated-IndexerControllerBase-IndexVideo-System-String,System-String,System-String- 'Keywords.API.Swagger.Controllers.Generated.IndexerControllerBase.IndexVideo(System.String,System.String,System.String)')
- [Keyword](#T-Keywords-API-Swagger-Controllers-Generated-Keyword 'Keywords.API.Swagger.Controllers.Generated.Keyword')
  - [Content](#P-Keywords-API-Swagger-Controllers-Generated-Keyword-Content 'Keywords.API.Swagger.Controllers.Generated.Keyword.Content')
  - [Id](#P-Keywords-API-Swagger-Controllers-Generated-Keyword-Id 'Keywords.API.Swagger.Controllers.Generated.Keyword.Id')
  - [IsPublished](#P-Keywords-API-Swagger-Controllers-Generated-Keyword-IsPublished 'Keywords.API.Swagger.Controllers.Generated.Keyword.IsPublished')
  - [VideoId](#P-Keywords-API-Swagger-Controllers-Generated-Keyword-VideoId 'Keywords.API.Swagger.Controllers.Generated.Keyword.VideoId')
- [KeywordControllerBase](#T-Keywords-API-Swagger-Controllers-Generated-KeywordControllerBase 'Keywords.API.Swagger.Controllers.Generated.KeywordControllerBase')
  - [GetKeyword(keywordId)](#M-Keywords-API-Swagger-Controllers-Generated-KeywordControllerBase-GetKeyword-System-Guid- 'Keywords.API.Swagger.Controllers.Generated.KeywordControllerBase.GetKeyword(System.Guid)')

<a name='T-Keywords-API-Swagger-Controllers-Generated-IndexerControllerBase'></a>
## IndexerControllerBase `type`

##### Namespace

Keywords.API.Swagger.Controllers.Generated

<a name='M-Keywords-API-Swagger-Controllers-Generated-IndexerControllerBase-GetOcrList-System-String-'></a>
### GetOcrList(videoId) `method`

##### Summary

Get a ocr list of the indexed video

##### Returns

Ocr video found

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| videoId | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Video Id to get the video ocr for |

##### Remarks

Get a ocr list of the indexed video

<a name='M-Keywords-API-Swagger-Controllers-Generated-IndexerControllerBase-IndexVideo-System-String,System-String,System-String-'></a>
### IndexVideo(url,name,description) `method`

##### Summary

Index a video

##### Returns

Video indexed

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| url | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Video url to index |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Video name to index |
| description | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Video description to index |

##### Remarks

Index a video

<a name='T-Keywords-API-Swagger-Controllers-Generated-Keyword'></a>
## Keyword `type`

##### Namespace

Keywords.API.Swagger.Controllers.Generated

<a name='P-Keywords-API-Swagger-Controllers-Generated-Keyword-Content'></a>
### Content `property`

##### Summary

Keyword's content can be a word or a phrase

<a name='P-Keywords-API-Swagger-Controllers-Generated-Keyword-Id'></a>
### Id `property`

##### Summary

Id of the keyword

<a name='P-Keywords-API-Swagger-Controllers-Generated-Keyword-IsPublished'></a>
### IsPublished `property`

##### Summary

States if the Keyword is available for users

<a name='P-Keywords-API-Swagger-Controllers-Generated-Keyword-VideoId'></a>
### VideoId `property`

##### Summary

Id of the video that the keyword belongs to

<a name='T-Keywords-API-Swagger-Controllers-Generated-KeywordControllerBase'></a>
## KeywordControllerBase `type`

##### Namespace

Keywords.API.Swagger.Controllers.Generated

<a name='M-Keywords-API-Swagger-Controllers-Generated-KeywordControllerBase-GetKeyword-System-Guid-'></a>
### GetKeyword(keywordId) `method`

##### Summary

Get a keyword by its id

##### Returns

Keyword found

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| keywordId | [System.Guid](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Guid 'System.Guid') | Keyword Id to get the keyword entity for |

##### Remarks

Get a keyword by its id
