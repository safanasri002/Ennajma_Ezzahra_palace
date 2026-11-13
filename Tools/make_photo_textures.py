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
