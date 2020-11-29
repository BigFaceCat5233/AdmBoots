﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Domain.Models {
    public class AuditLog : Entity {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// 服务 (类/接口) 名
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 执行方法名称
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 调用参数
        /// </summary>
        public string Parameters { get; set; }
        /// <summary>
        /// 返回值
        /// </summary>
        public string ReturnValue { get; set; }
        /// <summary>
        /// 方法执行的开始时间
        /// </summary>
        public DateTime ExecutionTime { get; set; }
        /// <summary>
        /// 方法调用的总持续时间（毫秒）
        /// </summary>
        public int ExecutionDuration { get; set; }
        /// <summary>
        /// 客户端的IP地址
        /// </summary>
        public string ClientIpAddress { get; set; }
        /// <summary>
        /// 客户端的名称（通常是计算机名）
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string BrowserInfo { get; set; }
        /// <summary>
        /// 方法执行期间发生异常
        /// </summary>
        public string Exception { get; set; }
        /// <summary>
        /// 自定义数据
        /// </summary>
        public string CustomData { get; set; }
    }
}
