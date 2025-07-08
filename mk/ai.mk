.PHONY: ai
ai: tmp/$(APP).ai.md
tmp/$(APP).ai.md: doc/*.md
	cat doc/*.md > $@ ; touch $@
