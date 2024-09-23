class HslColor {
    h = 0;
    s = 0;
    l = 0;

    constructor(h, s, l) {
        this.h = h;
        this.s = s;
        this.l = l;
    }
    static fromArr(hslArr) {
        return new HslColor(hslArr[0], hslArr[1], hslArr[2]);
    }
    static fromStr(hslStr) {
        if (hslStr.substring(0, 4) !== 'hsl(') return null;
        const [h, s, l] = hslStr.replace(/[^0-9\s]*/gm, '').split(' ');
        return new HslColor(h, s, l);
    }

    toArr() {
        return [this.h, this.s, this.l];
    }
    toStr() {
        return `hsl(${this.h} ${this.s} ${this.l})`;
    }

    toHex() {
        let h = this.h, s = this.s, l = this.l;
        l /= 100;
        const a = s * Math.min(l, 1 - l) / 100;
        const f = n => {
            const k = (n + h / 30) % 12;
            const color = l - a * Math.max(Math.min(k - 3, 9 - k, 1), -1);
            return Math.round(255 * color).toString(16).padStart(2, '0');   // convert to Hex and prefix "0" if needed
        };
        return `#${f(0)}${f(8)}${f(4)}`;
    }

    toRgb() {
        let r = 0, g = 0, b = 0;
        const l = this.l / 100, s = this.s / 100, h = this.h / 360;

        if (this.s === 0) {
            r = g = b = l; // achromatic
        } else {
            function hue2rgb(p, q, t) {
                if (t < 0) t += 1;
                if (t > 1) t -= 1;
                if (t < 1 / 6) return p + (q - p) * 6 * t;
                if (t < 1 / 2) return q;
                if (t < 2 / 3) return p + (q - p) * (2 / 3 - t) * 6;
                return p;
            }

            var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            var p = 2 * l - q;

            r = hue2rgb(p, q, h + 1 / 3);
            g = hue2rgb(p, q, h);
            b = hue2rgb(p, q, h - 1 / 3);
        }

        return [r * 255, g * 255, b * 255];
    }
};

class HexColor {
    hex = "";
    constructor(hexStr) {
        this.hex = hexStr;
    }

    toStr() {
        return this.hex;
    }

    toHsl() {
        // Convert hex to RGB first
        let r = 0, g = 0, b = 0;
        // Grab the hex representation of red (chars 1-2) and convert to decimal (base 10).
        if (this.hex.length == 4) {
            r = parseInt(this.hex.charAt(1) + this.hex.charAt(1), 16);
            g = parseInt(this.hex.charAt(2) + this.hex.charAt(2), 16);
            b = parseInt(this.hex.charAt(3) + this.hex.charAt(3), 16);
        } else if (this.hex.length == 7) {
            r = parseInt(this.hex.substring(1, 3), 16)
            g = parseInt(this.hex.substring(3, 5), 16);
            b = parseInt(this.hex.substring(5), 16);
        }
        // Then to HSL
        r /= 255;
        g /= 255;
        b /= 255;
        const max = Math.max(r, g, b);
        const min = Math.min(r, g, b);
        let h, s, l = (max + min) / 2;
        let d = max - min;

        if (d === 0) {
            h = s = 0; // achromatic
        } else {
            l = (max + min) / 2;
            s = d / (1 - Math.abs(2 * l - 1));
            switch (max) {
                case r: h = ((g - b) / d) % 6; break;
                case g: h = (b - r) / d + 2; break;
                case b: h = (r - g) / d + 4; break;
            }
        }

        h = Math.round(h * 60);
        if (h < 0) h += 360;
        s = +(s * 100).toFixed(0);
        l = +(l * 100).toFixed(0);
        return [h, s, l];
    }

    toRgb() {
        // Expand shorthand form (e.g. "03F") to full form (e.g. "0033FF")
        var shorthandRegex = /^#?([a-f\d])([a-f\d])([a-f\d])$/i;
        let hex = this.hex.replace(shorthandRegex, function (m, r, g, b) {
            return r + r + g + g + b + b;
        });

        var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
        return result ? [
            parseInt(result[1], 16),
            parseInt(result[2], 16),
            parseInt(result[3], 16)
        ] : null;
    }
};

class RgbaColor {
    r = 0;
    g = 0;
    b = 0;
    a = 0;
    constructor(r, g, b, a=1) {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    static fromArr(rgbaArr) {
        const r = rgbaArr[0];
        const g = rgbaArr[1];
        const b = rgbaArr[2];
        let a = 0;
        if (rgbaArr.length === 4)
            a = rgbaArr[3];
        return new RgbaColor(r, g, b, a);
    }

    toRgbArr() {
        return [this.r, this.g, this.b];
    }
    toStr() {
        return `rgba(${this.r} ${this.g} ${this.b} / ${this.a})`;
    }

    toHex() {
        return "#" + (1 << 24 | this[0] << 16 | this[1] << 8 | this[2]).toString(16).slice(1);
    }

    toHsl() {
        let r = this.r/255, g = this.g/255, b = this.b/255;

        const max = Math.max(r, g, b), min = Math.min(r, g, b);
        let h, s, l = (max + min) / 2;

        if (max == min) {
            h = s = 0; // achromatic
        } else {
            var d = max - min;
            s = l > 0.5 ? d / (2 - max - min) : d / (max + min);

            switch (max) {
                case r: h = (g - b) / d + (g < b ? 6 : 0); break;
                case g: h = (b - r) / d + 2; break;
                case b: h = (r - g) / d + 4; break;
            }

            h /= 6;
        }

        return [h * 360, s * 100, l * 100];
    }
};

Array.prototype.hslToHex = function () {
    let h = this[0], s = this[1], l = this[2];
    l /= 100;
    const a = s * Math.min(l, 1 - l) / 100;
    const f = n => {
        const k = (n + h / 30) % 12;
        const color = l - a * Math.max(Math.min(k - 3, 9 - k, 1), -1);
        return Math.round(255 * color).toString(16).padStart(2, '0');   // convert to Hex and prefix "0" if needed
    };
    return `#${f(0)}${f(8)}${f(4)}`;
};
String.prototype.strToHslArr = function () {
    if (this.substring(0, 4) !== 'hsl(') return [NaN, NaN, NaN];
    return this.replace(/[^0-9\s]*/gm, '').split(' ');
};
Array.prototype.arrToHslStr = function () {
    return `hsl(${this[0]} ${this[1]} ${this[2]})`;
};
String.prototype.hexToHsl = function () {
    let r = parseInt(color.substr(1, 2), 16); // Grab the hex representation of red (chars 1-2) and convert to decimal (base 10).
    let g = parseInt(color.substr(3, 2), 16);
    let b = parseInt(color.substr(5, 2), 16);
    r /= 255, g /= 255, b /= 255;
    const max = Math.max(r, g, b);
    const min = Math.min(r, g, b);
    let h, s, l = (max + min) / 2;

    if (max === min) {
        h = s = 0; // achromatic
    } else {
        let d = max - min;
        s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
        switch (max) {
            case r: h = (g - b) / d + (g < b ? 6 : 0); break;
            case g: h = (b - r) / d + 2; break;
            case b: h = (r - g) / d + 4; break;
        }
        h /= 6;
    }
    return [h * 360, s * 100, l * 100];
};
Array.prototype.rgbToHex = function () {
    return "#" + (1 << 24 | this[0] << 16 | this[1] << 8 | this[2]).toString(16).slice(1);
}
String.prototype.hexToRgb = function () {
    // Expand shorthand form (e.g. "03F") to full form (e.g. "0033FF")
    var shorthandRegex = /^#?([a-f\d])([a-f\d])([a-f\d])$/i;
    let hex = this.replace(shorthandRegex, function (m, r, g, b) {
        return r + r + g + g + b + b;
    });

    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? [
        parseInt(result[1], 16),
        parseInt(result[2], 16),
        parseInt(result[3], 16)
    ] : null;
}

const luminance = (rgbArr) => {
    const RED = 0.2126;
    const GREEN = 0.7152;
    const BLUE = 0.0722;
    const GAMMA = 2.4;

    let a = rgbArr.map((v) => {
        v /= 255;
        return v <= 0.03928
            ? v / 12.92
            : Math.pow((v + 0.055) / 1.055, GAMMA);
    });
    return a[0] * RED + a[1] * GREEN + a[2] * BLUE;
};

const contrast = (rgbArr1, rgbArr2) => {
    var lum1 = luminance(rgbArr1);
    var lum2 = luminance(rgbArr2);
    var brightest = Math.max(lum1, lum2);
    var darkest = Math.min(lum1, lum2);
    return (brightest + 0.05) / (darkest + 0.05);
}