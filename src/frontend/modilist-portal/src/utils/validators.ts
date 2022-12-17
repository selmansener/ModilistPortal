export function validateIdNumber(value?: string) {
    if (!value) {
        return false;
    }

    const invalidNumbers = [11111111110, 22222222220, 33333333330, 44444444440, 55555555550, 66666666660, 7777777770, 88888888880, 99999999990];
    if (invalidNumbers.some(x => x.toString() === value)) {
        return false;
    }

    const idNumber = value.split("").map(x => parseInt(x));

    // length must be 11, no more no less
    if (idNumber.length !== 11) {
        return false;
    }

    // first number can't be zero
    if (idNumber[0] === 0) {
        return false;
    }

    // get even and odd digits sum without last element
    const oddDigitsSum = idNumber.filter((x, i) => (i + 1) % 2 !== 0 && i !== 10).reduce((previous, current) => previous + current, 0);
    const evenDigitsSum = idNumber.filter((x, i) => (i + 1) % 2 === 0 && i !== 9).reduce((previous, current) => previous + current, 0);

    // get odds sum then multiply by 7 and substract even digits sum then divide by 10, remainder has to be equal to 10th element
    if (idNumber[9] !== ((oddDigitsSum * 7) - evenDigitsSum) % 10) {
        return false;
    }

    // sum all digits without last element and divide by 10, result has to be equal to last element
    if (idNumber[idNumber.length - 1] !== idNumber.filter((x, i) => i !== idNumber.length - 1).reduce((previous, current) => previous + current, 0) % 10) {
        return false;
    }

    return true;
}

export function validateTaxNumber(value?: string) {
    if (!value) {
        return false;
    }

    const taxNumber = value.split("").map(x => parseInt(x));

    let tmp = 0;
    let sum = 0;
    if (taxNumber != null && taxNumber.length == 10) {
        const lastDigit = taxNumber.at(9);
        for (let i = 0; i < 9; i++) {
            const digit = taxNumber.at(i) ?? 0;
            tmp = (digit + 10 - (i + 1)) % 10;
            sum = (tmp == 9 ? sum + tmp : sum + ((tmp * (Math.pow(2, 10 - (i + 1)))) % 9));
        }
        return lastDigit == (10 - (sum % 10)) % 10;
    }

    return false;
}