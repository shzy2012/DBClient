using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DataAccess {
    /// <summary>
    /// 记步排名 数据库操作
    /// </summary>
    public class StepRankingMongoDal {
        /// <summary>
        /// The connection string
        /// </summary>
        private string connection = ConfigurationManager.ConnectionStrings[ApplicationKeys.DbMongoConnect].ToString();
        /// <summary>
        /// The name of the db.
        /// </summary>
        private string dbName = ApplicationKeys.DbMongoData;
        /// <summary>
        /// The name of the collection.
        /// </summary>
        private string collection = "step_ranking";

        #region Method

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model">FriendsModel</param>
        /// <returns>true or false</returns>
        public bool Add(StepRankingModel model) {
            MongoClient client = new MongoClient(connection);
            var db = client.GetDatabase(this.dbName);
            var collection = db.GetCollection<StepRankingModel>(this.collection);
            object obj = ObjectIdGenerator.Instance.GenerateId(collection, model);
            model.Id = obj.ToString();
            //model.Id = Guid.NewGuid().ToString("N");
            //新增，则将第一次上传的步数赋值给总步数，后天更新是每次递增,下面一个方法中处理(UpdateModel)
            model.Total = model.Day;
            collection.InsertOneAsync(model);
            return true;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateModel(StepRankingModel model, int curTotal) {
            MongoClient client = new MongoClient(connection);
            var db = client.GetDatabase(this.dbName);
            var collection = db.GetCollection<StepRankingModel>(this.collection);
            //过滤条件
            var filter = Builders<StepRankingModel>.Filter.Where(x => x.ChildId == model.ChildId);
            //更新内容
            var update = Builders<StepRankingModel>.Update
                .Set("Day", model.Day)
                .Set("Date", model.Date)
                .Set("UpdateTime", model.UpdateTime)
                .Set("Total", curTotal);
            collection.UpdateOneAsync(filter, update);
            return true;
        }

        /// <summary>
        /// 通过childID获取 StepRankModel
        /// </summary>
        /// <param name="childId"></param>
        /// <returns>RankStepModel</returns>
        public StepRankingModel GetByChildId(int childId) {
            MongoClient client = new MongoClient(connection);
            var db = client.GetDatabase(this.dbName);
            var collection = db.GetCollection<StepRankingModel>(this.collection);
            //过滤条件
            var filter = Builders<StepRankingModel>.Filter.Where(x => x.ChildId == childId);
            return collection.Find(filter).FirstOrDefault();
        }


        /// <summary>
        /// 获取小孩一个月轨迹日期列表
        /// </summary>
        /// <param name="childId">小孩Id</param>
        /// <param name="beginTime">一个月的第一天</param>
        /// <param name="beginTime">一个月的最后一天</param>
        /// <returns></returns>
        public List<StepRankingModel> GetChildOneMonthTraceListImpl(int childId, DateTime begin, DateTime end) {
            MongoClient client = new MongoClient(connection);
            var db = client.GetDatabase(this.dbName);
            var collection = db.GetCollection<StepRankingModel>(this.collection);
            string beginTime = begin.ToString("yyyyMMdd");
            string endTime = end.ToString("yyyyMMdd");
            QueryDocument query = new QueryDocument();
            BsonDocument timeBson = new BsonDocument();
            timeBson.Add("$gt", beginTime);//>beginTime
            timeBson.Add("$lt", endTime);//<endTime
            query.Add("Date", timeBson);
            query.Add(new BsonElement("ChildId", childId));
            SortByDocument s = new SortByDocument();
            s.Add("Date", 1);//order by asc
            return collection.Find(query).Sort(s).ToList();
        }
        #endregion
    }
}
