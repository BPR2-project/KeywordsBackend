<a name='assembly'></a>
# Keywords.API.Swagger

## Contents

- [IndexerControllerBase](#T-Keywords-API-Swagger-Controllers-Generated-IndexerControllerBase 'Keywords.API.Swagger.Controllers.Generated.IndexerControllerBase')
  - [GetKeywordsFromIndexer(videoId)](#M-Keywords-API-Swagger-Controllers-Generated-IndexerControllerBase-GetKeywordsFromIndexer-System-Guid- 'Keywords.API.Swagger.Controllers.Generated.IndexerControllerBase.GetKeywordsFromIndexer(System.Guid)')
  - [IndexVideo(videoId,url)](#M-Keywords-API-Swagger-Controllers-Generated-IndexerControllerBase-IndexVideo-System-Guid,System-String- 'Keywords.API.Swagger.Controllers.Generated.IndexerControllerBase.IndexVideo(System.Guid,System.String)')
- [Keyword](#T-Keywords-API-Swagger-Controllers-Generated-Keyword 'Keywords.API.Swagger.Controllers.Generated.Keyword')
  - [AudioLink](#P-Keywords-API-Swagger-Controllers-Generated-Keyword-AudioLink 'Keywords.API.Swagger.Controllers.Generated.Keyword.AudioLink')
  - [Content](#P-Keywords-API-Swagger-Controllers-Generated-Keyword-Content 'Keywords.API.Swagger.Controllers.Generated.Keyword.Content')
  - [Id](#P-Keywords-API-Swagger-Controllers-Generated-Keyword-Id 'Keywords.API.Swagger.Controllers.Generated.Keyword.Id')
  - [IsPublished](#P-Keywords-API-Swagger-Controllers-Generated-Keyword-IsPublished 'Keywords.API.Swagger.Controllers.Generated.Keyword.IsPublished')
  - [Language](#P-Keywords-API-Swagger-Controllers-Generated-Keyword-Language 'Keywords.API.Swagger.Controllers.Generated.Keyword.Language')
  - [VideoId](#P-Keywords-API-Swagger-Controllers-Generated-Keyword-VideoId 'Keywords.API.Swagger.Controllers.Generated.Keyword.VideoId')
- [KeywordControllerBase](#T-Keywords-API-Swagger-Controllers-Generated-KeywordControllerBase 'Keywords.API.Swagger.Controllers.Generated.KeywordControllerBase')
  - [GetAllKeywordsByVideoId(paginatedKeywordsRequest)](#M-Keywords-API-Swagger-Controllers-Generated-KeywordControllerBase-GetAllKeywordsByVideoId-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsRequest- 'Keywords.API.Swagger.Controllers.Generated.KeywordControllerBase.GetAllKeywordsByVideoId(Keywords.API.Swagger.Controllers.Generated.PaginatedKeywordsRequest)')
  - [GetKeyword(keywordId)](#M-Keywords-API-Swagger-Controllers-Generated-KeywordControllerBase-GetKeyword-System-Guid- 'Keywords.API.Swagger.Controllers.Generated.KeywordControllerBase.GetKeyword(System.Guid)')
  - [PublishKeyword(keywordId,toBePublished)](#M-Keywords-API-Swagger-Controllers-Generated-KeywordControllerBase-PublishKeyword-System-Guid,System-Nullable{System-Boolean}- 'Keywords.API.Swagger.Controllers.Generated.KeywordControllerBase.PublishKeyword(System.Guid,System.Nullable{System.Boolean})')
- [PaginatedKeywordsRequest](#T-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsRequest 'Keywords.API.Swagger.Controllers.Generated.PaginatedKeywordsRequest')
  - [Page](#P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsRequest-Page 'Keywords.API.Swagger.Controllers.Generated.PaginatedKeywordsRequest.Page')
  - [Size](#P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsRequest-Size 'Keywords.API.Swagger.Controllers.Generated.PaginatedKeywordsRequest.Size')
  - [VideoId](#P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsRequest-VideoId 'Keywords.API.Swagger.Controllers.Generated.PaginatedKeywordsRequest.VideoId')
- [PaginatedKeywordsResponse](#T-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsResponse 'Keywords.API.Swagger.Controllers.Generated.PaginatedKeywordsResponse')
  - [CurrentPage](#P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsResponse-CurrentPage 'Keywords.API.Swagger.Controllers.Generated.PaginatedKeywordsResponse.CurrentPage')
  - [Keywords](#P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsResponse-Keywords 'Keywords.API.Swagger.Controllers.Generated.PaginatedKeywordsResponse.Keywords')
  - [SizeRequested](#P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsResponse-SizeRequested 'Keywords.API.Swagger.Controllers.Generated.PaginatedKeywordsResponse.SizeRequested')
  - [TotalAmount](#P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsResponse-TotalAmount 'Keywords.API.Swagger.Controllers.Generated.PaginatedKeywordsResponse.TotalAmount')
  - [TotalAmountOfPages](#P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsResponse-TotalAmountOfPages 'Keywords.API.Swagger.Controllers.Generated.PaginatedKeywordsResponse.TotalAmountOfPages')

<a name='T-Keywords-API-Swagger-Controllers-Generated-IndexerControllerBase'></a>
## IndexerControllerBase `type`

##### Namespace

Keywords.API.Swagger.Controllers.Generated

<a name='M-Keywords-API-Swagger-Controllers-Generated-IndexerControllerBase-GetKeywordsFromIndexer-System-Guid-'></a>
### GetKeywordsFromIndexer(videoId) `method`

##### Summary

Get a keyword list of the indexed video

##### Returns

Ocr video found

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| videoId | [System.Guid](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Guid 'System.Guid') | Video Id to get the video ocr for |

##### Remarks

Get a keyword list of the indexed video

<a name='M-Keywords-API-Swagger-Controllers-Generated-IndexerControllerBase-IndexVideo-System-Guid,System-String-'></a>
### IndexVideo(videoId,url) `method`

##### Summary

Index a video

##### Returns

Video index started

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| videoId | [System.Guid](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Guid 'System.Guid') | Video id to index |
| url | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Video url to index |

##### Remarks

Index a video

<a name='T-Keywords-API-Swagger-Controllers-Generated-Keyword'></a>
## Keyword `type`

##### Namespace

Keywords.API.Swagger.Controllers.Generated

<a name='P-Keywords-API-Swagger-Controllers-Generated-Keyword-AudioLink'></a>
### AudioLink `property`

##### Summary

Link to the keyword's audio

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

<a name='P-Keywords-API-Swagger-Controllers-Generated-Keyword-Language'></a>
### Language `property`

##### Summary

The language the keyword belongs to

<a name='P-Keywords-API-Swagger-Controllers-Generated-Keyword-VideoId'></a>
### VideoId `property`

##### Summary

Id of the video that the keyword belongs to

<a name='T-Keywords-API-Swagger-Controllers-Generated-KeywordControllerBase'></a>
## KeywordControllerBase `type`

##### Namespace

Keywords.API.Swagger.Controllers.Generated

<a name='M-Keywords-API-Swagger-Controllers-Generated-KeywordControllerBase-GetAllKeywordsByVideoId-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsRequest-'></a>
### GetAllKeywordsByVideoId(paginatedKeywordsRequest) `method`

##### Summary

Get paginated keywords for a video

##### Returns

Returns all keywords paginated

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| paginatedKeywordsRequest | [Keywords.API.Swagger.Controllers.Generated.PaginatedKeywordsRequest](#T-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsRequest 'Keywords.API.Swagger.Controllers.Generated.PaginatedKeywordsRequest') | Contains pagination details |

##### Remarks

Get paginated keywords for a video

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

<a name='M-Keywords-API-Swagger-Controllers-Generated-KeywordControllerBase-PublishKeyword-System-Guid,System-Nullable{System-Boolean}-'></a>
### PublishKeyword(keywordId,toBePublished) `method`

##### Summary

Publish a keyword using its id

##### Returns

Keyword published

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| keywordId | [System.Guid](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Guid 'System.Guid') | Keyword Id |
| toBePublished | [System.Nullable{System.Boolean}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Nullable 'System.Nullable{System.Boolean}') | Bool to state whether will be published or not |

##### Remarks

Publish keyboard using its id

<a name='T-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsRequest'></a>
## PaginatedKeywordsRequest `type`

##### Namespace

Keywords.API.Swagger.Controllers.Generated

<a name='P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsRequest-Page'></a>
### Page `property`

##### Summary

Page number

<a name='P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsRequest-Size'></a>
### Size `property`

##### Summary

Size of the page

<a name='P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsRequest-VideoId'></a>
### VideoId `property`

##### Summary

Video Id to get the keywords for

<a name='T-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsResponse'></a>
## PaginatedKeywordsResponse `type`

##### Namespace

Keywords.API.Swagger.Controllers.Generated

<a name='P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsResponse-CurrentPage'></a>
### CurrentPage `property`

##### Summary

Current page

<a name='P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsResponse-Keywords'></a>
### Keywords `property`

##### Summary

List of videos

<a name='P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsResponse-SizeRequested'></a>
### SizeRequested `property`

##### Summary

Size of the page that was requested

<a name='P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsResponse-TotalAmount'></a>
### TotalAmount `property`

##### Summary

Total number of keywords retrieved

<a name='P-Keywords-API-Swagger-Controllers-Generated-PaginatedKeywordsResponse-TotalAmountOfPages'></a>
### TotalAmountOfPages `property`

##### Summary

Total number of pages based on the specified size
