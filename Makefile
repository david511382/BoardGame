##########################################
## docker-compose
##########################################

DOCKER_SERVICE_LIST := member.db db.game redis.master redis-commander es01 es02 kibana

up: # debug 全開
	docker-compose -f ./deployment/docker/docker-compose.yml up --build -d

debug: # 只啟動開發需要的服務
	docker-compose -f ./deployment/docker/docker-compose.yml up -d \
	$(DOCKER_SERVICE_LIST)

ps: # docker-compose ps 查看
	docker-compose -f ./deployment/docker/docker-compose.yml ps

down: # 關閉
	docker-compose -f ./deployment/docker/docker-compose.yml down
