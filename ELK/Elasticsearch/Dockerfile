ARG ELK_VERSION

# https://github.com/elastic/elasticsearch-docker
FROM docker.elastic.co/elasticsearch/elasticsearch:${ELK_VERSION}

ADD ./config/elasticsearch.yml /usr/share/elasticsearch/config/
