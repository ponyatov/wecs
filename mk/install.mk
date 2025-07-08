.PHONY : install update ref gz
install: $(WS)_install $(RUSTUP) $(TSC) doc ref gz
	$(MAKE) update
update : $(WS)_update $(RUSTUP) $(TSC)
	$(RUSTUP) self update && $(RUSTUP) update
	$(NPM) update
ref    : $(RF)
gz     : $(GZ)

Debian_install:
# sudo dpkg --add-architecture i386
Debian_update:
	sudo apt update
	sudo apt install -uy `cat apt.$(WS)` $(APT)

Msys_install: doc ref gz
	pacman -Suy
Msys_update:
	pacman -S $(shell cat apt.$(WS) | tr '\n' ' ') $(MSYS)
