$(NPM):
	sudo apt install -uy npm nodejs
$(DENO): $(NPM)
	npm install -g deno
$(TSC): $(NPM)
	npm install -g typescript
