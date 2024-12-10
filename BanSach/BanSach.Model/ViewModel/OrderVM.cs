﻿namespace BanSach.Model.ViewModel
{
    public class OrderVM
    {
        public IEnumerable<OrderDetail>? OrderDetail { get; set; }
        public OrderHeader? OrderHeader { get; set; }
        public PagingInfo pagingInfo { get; set; } = new PagingInfo();

    }
}