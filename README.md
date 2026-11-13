# Projet Unity — Palais Ennejma Ezzahra (Sidi Bou Saïd)

Simulation de visite en vue à la première personne (caméra sur la tête du visiteur), en deux phases :

1. **Visualisation** : l'extérieur et l'intérieur andalou du palais.
2. **Chasse au trésor** : trouver la clé de la chambre secrète, le musée des instruments de musique anciens.

---

## Étape 1 (ce dossier) : socle du projet + Scène 1 « Extérieur »

### 1. Créer le projet
- **Unity 6 (6000.x)** recommandé, ou Unity 2022.3 LTS.
- Unity Hub → *New project* → template **Universal 3D (URP)** → nom : `EnnejmaEzzahra`.

### 2. Copier les fichiers
Copiez le dossier `Assets/` de cette archive **dans** le dossier `Assets/` de votre projet (fusionnez). Unity compile tout seul.

```
Assets/
├── Scripts/
│   ├── Player/  FirstPersonController.cs, InputBridge.cs
│   └── Core/    GameState.cs, ScreenFader.cs, SceneTransition.cs, SpawnPoint.cs, InfoZone.cs
└── Editor/
    ├── BuildKit.cs     (outils : murs, arcs andalous outrepassés, matériaux, joueur, lumière)
    ├── Props.cs        (palmiers, cyprès, orangers, maisons de Sidi Bou Saïd, moucharabiehs, fontaine…)
    └── Scenes/Scene01_ExteriorBuilder.cs
```

> Les entrées marchent avec le **nouvel Input System** (par défaut dans Unity 6) et avec l'ancien Input Manager.
> Avec l'Input System, les touches sont lues par position : sur un clavier AZERTY on se déplace avec **Z Q S D**.

### 3. Générer la scène
Menu **Ennejma → Phase 1 → Scène 1 - Extérieur**.
La scène est construite, enregistrée dans `Assets/Scenes/01_Exterieur.unity` et ajoutée aux *Build Settings*.
Les matériaux sont créés dans `Assets/Materials/Generated/` : vous pouvez changer leurs couleurs ou leur mettre des textures (photos, zellige…).

### 4. Jouer
Appuyez sur **Play**.

| Action | Touche |
|---|---|
| Avancer / reculer / côtés | Z Q S D (AZERTY) / W A S D (QWERTY) / flèches |
| Regarder | souris |
| Courir | Maj gauche |
| Libérer la souris | Échap (un clic pour la reprendre) |
| Interagir (phase 2) | E |

### Parcours de la Scène 1

```
                 N (+Z)
   ┌──────────────────────────────┬─────── parapet ──┐ z=96
   │                              │                  │
   │   PALAIS     façade est ►    │   ESPLANADE      │  ► vue panoramique :
   │  (x -62…-36)  PORTE (z=78) ══╪══►  motif zellige │    Sidi Bou Saïd,
   │                              │   palmiers, bancs │    phare, golfe,
   ├── fenêtres de l'étage ───────┤                  │    Bou Kornine
   │  COIN FONTAINE  bancs, pots  │                  │
   │   (fontaine en marbre)       │                  │z=55
   ├──────────────────────────────┘──── haie ─┐ A   │
   │                                         │ L   │ parapet
   │   JARDIN : palmiers, cyprès,            │ L   │  ► à droite :
   │   orangers, oliviers, massifs,          │ É   │    maisons blanches
   │   allées de gravier                     │ E   │    et bleues sur la pente
   │                                         │     │
   └──────── mur d'enceinte ─────────  [PORTE JAUNE] ┘ z=0
                                          RUE (départ)
```

1. **Rue** : départ devant la **porte jaune** (arc outrepassé, battants cloutés ouverts, bougainvilliers).
2. **Allée** : le jardin à gauche, le parapet et la vue sur Sidi Bou Saïd à droite.
3. **Détour à gauche** : l'**esplanade**, la vue panoramique, et en face la **façade principale**.
4. **Coin de la fontaine** : bancs de zellige, orangers, pots fleuris, avec les **moucharabiehs des chambres de l'étage** au-dessus.
5. **Porte du palais** : on la franchit et on passe à la Scène 2 (le message dans la Console est normal tant que la scène 2 n'existe pas).

Des **panneaux d'information** s'affichent à chaque étape de la visite (`InfoZone`).

---

## Scène 2 « Hall d'entrée »

Menu **Ennejma → Phase 1 → Scène 2 - Hall d'entrée** (ou en ligne de commande, Unity fermé) :
```
"C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Unity.exe" -batchmode -quit -projectPath . -executeMethod Ennejma.EditorTools.Scene02_HallBuilder.Build -logFile build.log
```

```
                 BAS-CÔTÉ NORD : escalier ▲ (étage), divan rouge, niche de marbre noir
        ┌──── arcades sur colonnes jumelées de marbre rose ─────────────────────┐
 BIT    │ porte     GRANDE NEF (2 niveaux)          vasque ◄ rigole ◄  DESCENTE ◄ VESTIBULE ◄ PORTE (scène 1)
 GAÂD ◄─┤ rosaces   damier rose et blanc, galerie haute, plafond doré  8 marches
        └──── arcades sur colonnes jumelées de marbre rose ─────────────────────┘
                 BAS-CÔTÉ SUD : escalier ▲ (étage), divans rouges
```

1. **Vestibule** : porte cloutée vert-de-gris, arc blanc, lanterne de verre suspendue.
2. **Descente** : 8 marches de marbre rose vers la nef en contrebas.
3. **Grande nef** : sol en damier rose et blanc, rigole d'eau et **vasque** à veilleuse, arcs outrepassés sur
   **colonnes jumelées** de marbre rose, murs de **stuc ciselé**, **galerie haute** à balustrades de bois tourné,
   **plafond de bois doré**, lanterne de bronze.
4. **Bas-côtés** : lambris d'**onyx rouge**, divans, niche de marbre noir, lampes de bronze, et les **deux escaliers**
   de marbre rose vers l'étage (scène 3, à venir).
5. **Au fond** : la porte du **bit gaâd** entre deux rosaces de stuc ; au-dessus, la galerie de **moucharabiehs** rouges.
   Dans le bit gaâd : divans, plateau de cuivre, lambris de marbre, fenêtre et vitrail en arc.

Les textures de stuc, d'onyx, de marbre noir, du plafond doré et des moucharabiehs sont recadrées depuis des photos
du palais (`Tools/make_photo_textures.py`). Captures de contrôle : menu **Ennejma → Outils → Captures - Scène 2**
(images dans `Screenshots/`).

---

## Feuille de route (scène par scène)

| # | Scène | Contenu | Phase |
|---|---|---|---|
| 1 | `01_Exterieur` | Porte jaune, allée, jardin, vue, esplanade, fontaine, façade | 1 ✅ |
| 2 | `02_Hall_Entree` | Vestibule, grand hall au plafond très haut sculpté, salons arabesques (*bit gaâd*), départ de l'escalier | 1 + début de la chasse ✅ |
| 3 | `03_Etage_Chambres` | Escalier de marbre, couloir, chambres à coucher (vue jardin / vue Sidi Bou Saïd) | 1 |
| 4 | `04_Chambre_Baron` | Grande chambre du baron (aile est) | 1 |
| 5 | `05_Salle_Peintures` | Ses peintures de Sidi Bou Saïd et de Tunis (aile ouest) | 1 |
| 6 | `06_Salle_Fetes` | Couloir vers la salle des fêtes musicales | 1 |
| 7 | Système de chasse | Indices, inventaire, inspection d'objets, clé, porte verrouillée | 2 |
| 8 | `07_Couloir_Secret` + `08_Musee_Instruments` | Découverte du musée : instruments, livres, documents anciens | 2 |

## Pour aller plus loin (optionnel)
- Remplacer les primitives par de vrais modèles (Blender, ProBuilder, Asset Store) en gardant les mêmes positions.
- Ajouter des sons d'ambiance (oiseaux, fontaine, oud au loin) avec des `AudioSource`.
- Faire cuire l'éclairage (*Window → Rendering → Lighting → Generate*) pour de belles ombres douces.

## Dépannage
- **Le visiteur tombe ou ne bouge pas** : vérifiez qu'un seul objet `Visiteur` est dans la scène, avec le tag *Player*.
- **Matériaux roses** : le projet n'est pas en URP ou en Built-in standard. Relancez le menu de construction après avoir changé de pipeline.
- **Erreur d'Input** : *Edit → Project Settings → Player → Active Input Handling* → **Both**.
