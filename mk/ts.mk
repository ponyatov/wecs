$(NPM):
	sudo apt install -uy npm nodejs
$(DENO): $(NPM)
	npm install -f deno
