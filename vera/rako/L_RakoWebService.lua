module ("L_RakoWebSerivce", package.seeall)

deviceSID = "urn:nickrandell-com:serviceId:RakoWebService1"
rwsDeviceSettings = {}

LOG_NAME = "RakoWebService: "

-- http://192.168.1.104:3480/data_request?id=variableget&DeviceNum=13&Variable=Armed&serviceId=urn:micasaverde-com:serviceId:SecuritySensor1

-- Builds the request and sends off to our service
function SendCommand(lul_device, lul_settings)

    local response_body = { }
    local url = "http://" .. rwsDeviceSettings.RakoHost .. "/rako.cgi?room=" .. lul_settings.Room .. "&com=" .. lul_settings.Command

    luup.log(LOG_NAME.."SendRequest url "..url,25)

    local statusCode, content = luup.inet.wget(url)

end

-- Hat tip to the SMTP plugin for gettings vars like this
function rwsReadVariable(lul_device, devicetype, name, defaultValue)

    local var = luup.variable_get(devicetype,name, lul_device)

    if (var == nil) then
        var = defaultValue
        luup.variable_set(devicetype,name,var,lul_device)
    end

    -- watch for those ampersands run through the UIs
    var = string.gsub(var,"&amp;","&")

    return var

end

-- Read in our settings variables. As at v1.0 there is no convenience interface
-- to enter these into Vera. Instead add them as custom variables in the
-- Advanced tab using:
--     urn:nickrandell-com:serviceId:RakoWebService1
-- as the "New Service" identifier
function rwsStartup(lul_device)
    luup.log("Rako startup")

    luup.log(LOG_NAME.." startup device "..lul_device,25)

    rwsDeviceSettings.RakoHost = rwsReadVariable(lul_device,deviceSID,"RakoHost","rako.home")
end
