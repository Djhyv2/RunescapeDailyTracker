module.exports = class Utilities
{
    //If array exists, push element to it, else return new array
    static createOrPush(array, element)
    {
        if (array)
        {
            array.push(element);
            return array;
        }

        return [element];
    }

    //If object has key do nothing, else set that key to value
    static setIfNull(object, key, value)
    {
        if (object[key])
        {
            return;
        }
        //eslint-disable-next-line no-param-reassign
        object[key] = value;
    }
};
