﻿using Ecmanage.eProcessor.Services.FakeFetch.FakeFetch.Domain.Entities.EmailTemplates;

namespace Ecmanage.eProcessor.Services.FakeFetch.FakeFetch.Application.Common.Models;

public class ReportDto : XmlDataDto
{
    public string PortalName { get; init; } = null!;
    public string ReportName { get; init; } = null!;
    public string Url { get; init; } = null!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Report, ReportDto>().ReverseMap();
        }
    }
}
