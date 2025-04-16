﻿namespace SharedLibrary.Requests;

public class GetUserTransactionsRequest
{
    public Guid UserId { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
