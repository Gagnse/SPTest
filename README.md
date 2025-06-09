# SpaceLogic
👉 http://localhost:5173 (frontend en mode dev)
👉 http://localhost:5000/weatherforecast (backend API)
👉 http://localhost:5000/swagger
http://localhost:5050/login?next=/browser/ avec user : admin@admin.com et password : admin

Commandes Docker utiles
docker compose up	: Démarre les conteneurs (sans rebuild)
docker compose up --build	: Recompile les images si on a modifié du code (backend, frontend etc.)
docker compose down	: Arrête tous les conteneurs
docker compose restart backend	: Redémarre uniquement le backend
docker compose logs -f backend	: Affiche les logs en temps réel du backend
ctr c : pour quitter

Pour les migrations
Dans un terminal à la racine du projet : 
docker compose run --rm backend bash
Puis dans le conteneur temporaire :
export PATH="$PATH:/root/.dotnet/tools"
dotnet tool install --global dotnet-ef (si besoin d'installer ef)
dotnet ef migrations add InitialCreate (si migration initiale)
dotnet ef migrations add MaMigration (pour ajouter une migration)
dotnet ef database update

Pour vérifier les BD dans pgAdmin
Aller sur http://localhost:5050
Se connecter :
Email : admin@admin.com
Mot de passe : admin

Pour les tests unitaires backend
dans le terminal, aller dans le dossier backend.Tests
Faire la commande :
dotnet test

Pour vérifier les erreurs TypeScript
npm run build

Pour vérifier les erreurs linter 
npm run lint

