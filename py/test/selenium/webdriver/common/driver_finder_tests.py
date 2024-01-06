# Licensed to the Software Freedom Conservancy (SFC) under one
# or more contributor license agreements.  See the NOTICE file
# distributed with this work for additional information
# regarding copyright ownership.  The SFC licenses this file
# to you under the Apache License, Version 2.0 (the
# "License"); you may not use this file except in compliance
# with the License.  You may obtain a copy of the License at
#
#   http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing,
# software distributed under the License is distributed on an
# "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
# KIND, either express or implied.  See the License for the
# specific language governing permissions and limitations
# under the License.
from pathlib import Path
from unittest import mock

import pytest

from selenium import webdriver
from selenium.common.exceptions import NoSuchDriverException
from selenium.webdriver.common.driver_finder import DriverFinder


def test_get_results_with_valid_path():
    options = webdriver.ChromeOptions()
    service = webdriver.ChromeService(executable_path="/valid/path/to/driver")

    with mock.patch.object(Path, 'is_file', return_value=True):
        result = DriverFinder.get_result(service, options)
    assert result == {"driver_path": "/valid/path/to/driver"}


def test_errors_with_invalid_path():
    options = webdriver.ChromeOptions()
    service = webdriver.ChromeService(executable_path="/invalid/path/to/driver")

    with mock.patch.object(Path, 'is_file', return_value=False):
        with pytest.raises(NoSuchDriverException) as excinfo:
            DriverFinder.get_result(service, options)
        assert "Unable to obtain driver for chrome; For documentation on this error" in str(excinfo.value)


def test_wraps_error_from_se_manager():
    options = webdriver.ChromeOptions()
    service = webdriver.ChromeService(executable_path="/valid/path/to/driver")

    lib_path = "selenium.webdriver.common.selenium_manager.SeleniumManager"
    with mock.patch(lib_path + '.result', side_effect=Exception("Error")):
        with pytest.raises(NoSuchDriverException):
            DriverFinder.get_result(service, options)


def test_get_results_from_se_manager(monkeypatch):
    options = webdriver.ChromeOptions()
    service = webdriver.ChromeService(executable_path="/invalid/path/to/driver")
    expected_output = {"driver_path": "/invalid/path/to/driver"}
    monkeypatch.setattr(Path, "is_file", lambda _: True)

    lib_path = "selenium.webdriver.common.selenium_manager.SeleniumManager"
    with mock.patch(lib_path + '.result', return_value=expected_output):
        result = DriverFinder.get_result(service, options)
    assert result == expected_output
