"""
Fabrique les textures « photo » du palais à partir de Reference_Photos/20_visite_milestone2/.
Les photos sources restent locales ; seules les textures recadrées sont écrites dans Assets/Art/Textures/Photos/.

Usage (depuis la racine du projet) :  python Tools/make_photo_textures.py
"""
from pathlib import Path
from PIL import Image, ImageOps

SRC = Path("Reference_Photos/20_visite_milestone2")
DST = Path("Assets/Art/Textures/Photos")
DST.mkdir(parents=True, exist_ok=True)


def load(n):
    return Image.open(SRC / f"{n}.jpg").convert("RGB")


def mirror_tile(img, size):
    """Rend un recadrage raccordable : image + miroirs (2 x 2), puis redimensionne."""
    w, h = img.size
    out = Image.new("RGB", (w * 2, h * 2))
    out.paste(img, (0, 0))
    out.paste(ImageOps.mirror(img), (w, 0))
    out.paste(ImageOps.flip(img), (0, h))
    out.paste(ImageOps.flip(ImageOps.mirror(img)), (w, h))
    return out.resize(size, Image.LANCZOS)


def mirror_tile_h(img, size):
    w, h = img.size
    out = Image.new("RGB", (w * 2, h))
    out.paste(img, (0, 0))
    out.paste(ImageOps.mirror(img), (w, 0))
    return out.resize(size, Image.LANCZOS)


def quad(img, corners, size):
    """Redresse un quadrilatère (HG, BG, BD, HD) en rectangle (perspective)."""
    (x0, y0), (x1, y1), (x2, y2), (x3, y3) = corners
    return img.transform(size, Image.QUAD, (x0, y0, x1, y1, x2, y2, x3, y3), Image.BICUBIC)


def save(img, name):
    img.save(DST / f"{name}.jpg", quality=90, optimize=True)
    print("ok", name, img.size)


# Rosace de stuc ciselé (photo frontale)
save(load(63).crop((40, 40, 1170, 1170)).resize((1024, 1024), Image.LANCZOS), "Photo_Rosace_Stuc")
# Treillis de stuc (fond de la rosace), rendu raccordable
treillis = mirror_tile(load(63).crop((60, 60, 230, 230)), (512, 512))
# Éclaircir : le stuc est crème sur place (la photo est prise à contre-jour)
save(Image.blend(treillis, Image.new("RGB", treillis.size, (238, 230, 212)), 0.45), "Photo_Stuc_Treillis")
# Plafond doré de la Grande chambre dorée : panneau central étoilé, redressé
p101 = load(101)
save(quad(p101, [(160, 0), (270, 255), (985, 255), (1100, 0)], (1024, 384)), "Photo_Plafond_Dore")
save(mirror_tile_h(p101.crop((210, 372, 1060, 470)), (1024, 128)), "Photo_Frise_Doree")
save(mirror_tile_h(p101.crop((200, 478, 1080, 528)), (1024, 64)), "Photo_Frise_Stuc_Calligraphie")
# Galerie de moucharabiehs en bois rouge (photo frontale)
save(load(86).crop((90, 40, 815, 495)).resize((1024, 640), Image.LANCZOS), "Photo_Moucharabieh_Rouge")
# Marbres de l'escalier : onyx rouge et marbre noir
p89 = load(89)
save(mirror_tile(p89.crop((40, 275, 160, 465)), (512, 512)), "Photo_Onyx_Rouge")
save(mirror_tile(p89.crop((752, 365, 818, 475)), (256, 256)), "Photo_Marbre_Noir")

# ---------------------------------------------------------------- Extérieur (photos de la visite)
EXT = Path("Reference_Photos")


def load_ext(rel):
    return Image.open(EXT / rel).convert("RGB")


# Battant gauche de la porte jaune, vu de face (photo 02) : clous, croissant, médaillons
p02 = load_ext("01_exterieur_allee_porte/02_porte_jaune_cloutee_arc_stucs.jpg")
save(p02.crop((50, 300, 318, 1015)).resize((384, 1024), Image.LANCZOS), "Photo_Porte_Jaune")
# Bandeau de pierre sculptée au-dessus de l'arc du portail (rosaces, cadres emboîtés)
save(p02.crop((0, 45, 720, 235)).resize((1024, 270), Image.LANCZOS), "Photo_Portail_Bandeau")
# Montant sculpté du portail (panneaux emboîtés, à droite de l'arc)
save(p02.crop((570, 560, 690, 1000)).resize((256, 940), Image.LANCZOS), "Photo_Portail_Montant")
# Dalles de pierre grise de la rue devant le portail
save(mirror_tile(p02.crop((0, 1060, 720, 1280)), (1024, 512)), "Photo_Dalles_Rue")
# Bassin des cyprès (photo 10) : frise de zellige vert et jaune, dalles de calcaire
p10 = load_ext("02_jardin_bassin/10_bassin_cypres.jpg")
save(mirror_tile_h(p10.crop((20, 848, 700, 879)), (1024, 48)), "Photo_Frise_Zellige_Bassin")
save(mirror_tile(p10.crop((0, 905, 774, 1032)), (1024, 512)), "Photo_Dalles_Calcaire")
