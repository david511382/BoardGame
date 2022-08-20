
##########################################
## docker-compose
##########################################

up: # debug 全開
	docker-compose -f ./deployment/docker/docker-compose.yml up -d

debug: # 只啟動開發需要的服務
	docker-compose -f ./deployment/docker/docker-compose.yml stop redis-commander
	docker-compose -f ./deployment/docker/docker-compose.yml up -d \
	member.db db.game redis.master redis-commander es01 es02 kibana

ps: # docker-compose ps 查看
	docker-compose -f ./deployment/docker/docker-compose.yml ps

down: # 關閉
	docker-compose -f ./deployment/docker/docker-compose.yml down
