# Rapport d'écarts — scène `01_Exterieur` face à la réalité

Les écarts sont classés par **priorité** : P1 = l'agencement est faux, P2 = un élément majeur manque, P3 = une forme ou une dimension est fausse, P4 = une matière ou un détail est faux.

Les corrections d'agencement (P1) ne seront faites **qu'après validation**, et certaines attendent la vue satellite.

Les numéros de photos renvoient à `Reference_Photos/INDEX.md` (W = photo du web) ; les dimensions, à `REALITE_DIMENSIONS.md` ; les hypothèses (G…, L…), à `REALITE_HYPOTHESES.md`.

## P1 — Agencement, orientation, relief

| # | Écart | Scène actuelle | Réalité (référence) | Correction proposée |
|---|---|---|---|---|
| 1 | **Relief** | terrain plat à y = 0 ; une seule pente, à l'est de l'allée | trois niveaux en terrasses ; le palais forme deux « marches » ; l'allée court au pied d'un mur de soutènement de 4 à 5 m (photo 23) ; on la voit en contrebas depuis l'esplanade (photos 11, 12, 19 et 20) | refaire le terrain en terrasses d'après la vue satellite et les photos : niveau de l'allée, niveau du palais et de l'esplanade, pente boisée vers la mer |
| 2 | **Vue vers la mer** | le village de Sidi Bou Saïd (colline, minaret, phare) est en face, à l'est | le village est **derrière** le palais (G8) ; vers la mer, on voit le **port de plaisance**, la **plage en arc** bordée de palmiers et des toits plats en contrebas (photos 03, 08, 15, 22 et 25) | supprimer la « colline du village » à l'est ; poser le port, ses jetées et ses mâts, la plage et les maisons en contrebas, aux bonnes directions |
| 3 | **Bou Kornine** | à 0,9 km à l'**est–sud-est** (azimut ≈ 118°), sous la forme de deux sphères | à **18,3 km plein sud** (≈ 184°), 576 m, deux cornes dans la brume (photo 03, G5) | silhouette fidèle à la bonne distance (imposteur) ; ajouter les montagnes du **cap Bon** à l'est–sud-est (G6) |
| 4 | **Bassin des cyprès (jardin andalou)** | absent ; à sa place, un « jardin » de palmiers et d'orangers posés au hasard | bassin de **2,5 × 45 m**, trois rangées de cyprès de chaque côté, murets de pierre sèche, Belvédère au bout (photos 10, 13 et 16 ; CMAM) | le construire sur le niveau haut, sur l'axe Belvédère–minaret (G9) ; supprimer le jardin aléatoire |
| 5 | **Jardin persan** | « coin de la fontaine » inventé : fontaine octogonale verte, 4 bancs de zellige, haies, orangers | quatre carrés à grilles bleues avec bigaradiers, vasque de grès au centre, porte bleue sous un auvent demi-cercle, lions, colonnes, chebbek, kiosque à tuiles vertes (photos 28 à 45, W71, W82, W92 ; G10) | le refaire entièrement d'après les photos |
| 6 | **Skifa** | absente : la porte jaune s'ouvre directement sur l'allée | passage voûté en brique d'environ 5 m, avec porte et banquette bleues (photo 02, W76) | l'ajouter derrière la porte jaune |
| 7 | **Parcours** porte jaune → allée → esplanade → palais | allée droite vers le nord, puis virage à gauche | ordre et sens de la montée ou de la descente **inconnus** (G11) | **à décider avec la vue satellite** ; garder les transitions et les SpawnPoints, et ne déplacer l'entrée qu'après validation |
| 8 | **Soleil** | azimut ≈ 65° (nord-est), hauteur 42° : un matin d'été | photos prises vers 15 h – 16 h 30 en hiver : azimut 220–235°, hauteur 10–20° (L1 à L3) | composant `SunPositionSetter` (phase B ou E), réglé sur la date et l'heure confirmées |

## P2 — Éléments majeurs manquants

| # | Écart | Réalité | Correction |
|---|---|---|---|
| 9 | **Galerie à arcades** devant la façade principale | 11 travées d'arcs **plein cintre** sur fines colonnes de marbre, dallage de marbre veiné, environ 3 m de profondeur (photo 41, W58, W69, W75, W94, W98) | la construire (MeshKit ArcadeWall + colonnes) |
| 10 | **Balustrade de l'esplanade** | piliers blancs d'environ 0,95 m tous les 3 m, balustrade bleue à barreaux (photos 32 et 41) | remplacer le parapet plein à chaperon bleu |
| 11 | **Piliers à lanterne** | piliers blancs à chapeau pyramidal, lanterne de fer accrochée (photos 22, 25 et 27) | remplacer les lanternes sur mât |
| 12 | **Mur de soutènement** du palais au-dessus de l'allée, avec le drapeau | photo 23 | voir l'écart 1 |
| 13 | **Escalier latéral en marbre** avec auvent bleu et balustrade (photo 24) | — | l'ajouter quand l'agencement sera fixé |
| 14 | **Coupole blanche** et **drapeau tunisien** sur le toit, visibles derrière la façade | W94, W95, W98 | le drapeau manque ; les coupoles sont à revoir (taille et place) |
| 15 | **Pente boisée** en contrebas | dense, avec eucalyptus en têtards, agaves, oliviers sauvages, acacias, figuiers de Barbarie (photos 11, 12, 19 à 21, 26 ; CMAM) | voir la phase F |

## P3 — Formes et dimensions

| # | Écart | Scène | Réalité | Correction |
|---|---|---|---|---|
| 16 | Hauteur de la façade principale | 12,5 m | ≈ 10,5 m | réduire ; le bandeau d'étage passe à ≈ 5,2 m |
| 17 | Porte principale | 3,2 m de large, au nu de la façade | ≈ 2,2 × 3,4 m, au fond de la galerie | réduire et la reculer derrière la galerie |
| 18 | Étage de la façade | une rangée régulière de 7 moucharabiehs | fenêtres à **volets persiennés bleus**, **2 ou 3 chebbeks** seulement, une **loggia centrale** à encadrement bleu (W94, W98) | refaire la composition d'après W98 |
| 19 | Passage de la porte jaune | 3,0 m | ≈ 2,2 m | réduire |
| 20 | Portail de la porte jaune | toit de tuiles vertes, alfiz simple | **cadre de pierre sculptée** à cadres emboîtés et rosaces, sans tuiles vertes visibles (photo 02, W66, W76) | refaire l'encadrement (phase C) |
| 21 | Entraxe des cyprès | 5 m, une seule rangée | ≈ 3 m, trois rangées de chaque côté du bassin | voir l'écart 4 |
| 22 | Fontaine | octogone de 4,4 m | vasque de grès d'environ 1 m | voir l'écart 5 |
| 23 | Esplanade : motif central d'étoile de zellige | présent | **absent** des photos (sol en béton rose uni à joints sciés) | supprimer |
| 24 | Palmiers en jardinières sur l'esplanade, bancs de zellige | présents | absents des photos de l'esplanade (W94, W98) | supprimer, ou les déplacer là où les photos en montrent |

## P4 — Matières et détails

| # | Écart | Scène | Réalité | Correction |
|---|---|---|---|---|
| 25 | Sol de l'esplanade et des allées | blanc ou beige uni | **béton rose** à joints, bordures blanches (photos 01, 24 et 32) | matériau rose, PBR calibré (phase D) |
| 26 | Rue et seuil de la porte jaune | pavés uniformes | grandes dalles de pierre grise irrégulières (photo 02) | phase D |
| 27 | Blanc des façades | blanc pur, sans variation | chaux d'albédo 0,80–0,85, coulures et salissures en pied de mur (photos 23, 35, W94) | phase D (decals) |
| 28 | Bleu | un seul bleu saturé | deux bleus : bois peint mat (volets, chebbeks) et fer forgé laqué (grilles) | phase D |
| 29 | Zellige | sur les bancs et les lambris de la façade | **frise de zellige vert et jaune** autour du bassin ; panneaux turquoise dans le jardin persan (photo 45) ; aucun lambris de zellige sur la façade principale | déplacer |
| 30 | Végétation | palmiers et orangers en sphères, haies en boîtes | eucalyptus en têtards, cyprès anciens, palmier des Canaries (photos 09 et 14), bougainvillier magenta, géraniums, agaves | phase F |
| 31 | Ciel et atmosphère | ciel bleu saturé, sans brume | horizon laiteux, mer bleu-gris, lointains désaturés (photos 03, 21, 26, 30) | phase E |
| 32 | Plaque « ENNEJMA EZZAHRA » bleue à droite de la porte | inventée | une plaque existe **à gauche** du portail (W66), mais son texte est illisible | photo de près demandée |

## Ce qui n'a pas d'écart identifié
- La largeur de l'allée (≈ 4 m).
- La longueur de la façade (≈ 36 m).
- La taille des chebbeks.
- Le principe d'une entrée par la façade côté mer, avec une transition vers le hall.

## Informations demandées à l'autrice (bloquantes pour la phase C)
1. **Capture satellite** (Google Maps ou Google Earth) du domaine : allées, bassin, jardin persan, esplanade, porte jaune. Deux captures si possible, l'une à l'échelle du domaine, l'autre à l'échelle du quartier jusqu'au port.
2. La **date et l'heure** de ta visite (L1 et L2).
3. Le **parcours réel** : en entrant par la porte jaune, on **monte** ou on **descend** vers le palais ? Dans quel ordre passe-t-on l'allée, le bassin, l'esplanade et le jardin persan ?
4. Des photos supplémentaires si possible : la skifa vue de l'intérieur, la façade principale de face, la porte jaune de face.
