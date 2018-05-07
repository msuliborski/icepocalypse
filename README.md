# Icepocalypse
download:   
            git pull origin [branch]

upload:     
            git add .
            git commit -m "wiadomość"
            git push origin [branch]

Variables:
            public: PascalCase
            private: _camelCase

Branches:
            git checkout -b [branch]
            git checkout [branch]
			
Tasks:

			background
			player:
				reflection jump
				climb
				idle
				simple jumprun
				attack
				crouch
			enemy:
				idle
				run
				attack
			buildings:
				windows
				doors
				snow
