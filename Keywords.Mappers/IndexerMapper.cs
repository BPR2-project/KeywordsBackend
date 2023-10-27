﻿using AutoMapper;
using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Mappers;

public class IndexerMapper: Profile
{
    public IndexerMapper()
    {
        CreateMap<indexer_api.Ocr, Ocr>();
    }
}