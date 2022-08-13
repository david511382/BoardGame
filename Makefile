
##########################################
## docker-compose
##########################################

up: # debug 全開
	docker-compose -f ./deployment/docker/docker-compose.yml up -d

down: # 關閉
	docker-compose -f ./deployment/docker/docker-compose.yml down
