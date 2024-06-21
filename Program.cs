﻿using System;
using System.Runtime.InteropServices;
using WebGpu;
using static WebGpu.Interop;
using WGpuAdapter = WebGpu.WGpuObjectBase;
using WGpuCanvasContext = WebGpu.WGpuObjectBase;
using WGpuDevice = WebGpu.WGpuObjectBase;
using WGpuQueue = WebGpu.WGpuObjectBase;
using WGpuRenderPipeline = WebGpu.WGpuObjectBase;
using WGpuCommandEncoder = WebGpu.WGpuObjectBase;
using WGpuRenderPassEncoder = WebGpu.WGpuObjectBase;
using WGpuCommandBuffer = WebGpu.WGpuObjectBase;
using WGpuShaderModule = WebGpu.WGpuObjectBase;


public static class Program
{
    private static WGpuAdapter adapter;
    private static WGpuCanvasContext canvasContext;
    private static WGpuDevice device;
    private static WGpuQueue queue;
    private static WGpuRenderPipeline renderPipeline;

    static void Main(string[] args)
    {
        Console.WriteLine("Hello from C#!");
        StartCalls();
    }

    private static unsafe void StartCalls()
    {
        int test = navigator_gpu_get_preferred_canvas_format();
    }

    /*
    private static unsafe void StartCalls()
    {
        int test = navigator_gpu_get_preferred_canvas_format();

        WGpuRequestAdapterOptions options = default;
        options.powerPreference = WGPU_POWER_PREFERENCE_LOW_POWER;
        navigator_gpu_request_adapter_async(ref options, &ObtainedWebGpuAdapter, IntPtr.Zero);
    }

    [UnmanagedCallersOnly]
    static unsafe void ObtainedWebGpuAdapter(WGpuAdapter result, IntPtr userData)
    {
        adapter = result;

        if (adapter.ptr == IntPtr.Zero)
            Console.WriteLine("ObtainedWebGpuAdapter was given a null GpuAdapter :-(");

        WGpuDeviceDescriptor deviceDesc = default;
        //TODO deviceDesc In/Out or ref
        wgpu_adapter_request_device_async(adapter, ref deviceDesc, &ObtainedWebGpuDevice, IntPtr.Zero);
    }

    [UnmanagedCallersOnly]
    static unsafe void ObtainedWebGpuDevice(WGpuDevice result, IntPtr userData)
    {
        if (result.ptr == IntPtr.Zero)
            Console.WriteLine("ObtainedWebGpuDevice was given device " + result.ptr);

        device = result;
        queue = wgpu_device_get_queue(device);
        canvasContext = wgpu_canvas_get_webgpu_context("canvas");

        WGpuCanvasConfiguration config = GetWGPU_CANVAS_CONFIGURATION_DEFAULT_INITIALIZER();
        config.device = device;
        config.format = navigator_gpu_get_preferred_canvas_format();
        wgpu_canvas_context_configure(canvasContext, ref config);

        string vertexShader =
            "@vertex\n" +
            "fn main(@builtin(vertex_index) vertexIndex : u32) -> @builtin(position) vec4<f32> {\n" +
            "var pos = array<vec2<f32>, 3>(\n" +
            "vec2<f32>(0.0, 0.5),\n" +
            "vec2<f32>(-0.5, -0.5),\n" +
            "vec2<f32>(0.5, -0.5)\n" +
            ");\n" +

        "return vec4<f32>(pos[vertexIndex], 0.0, 1.0);\n" +
        "}\n";

        string fragmentShader =
            "@fragment\n" +
        "fn main() -> @location(0) vec4<f32> {\n" +
        "return vec4<f32>(1.0, 0.5, 0.3, 1.0);\n" +
        "}\n";

        WGpuShaderModuleDescriptor shaderModuleDesc = default;
        shaderModuleDesc.code = vertexShader;
        WGpuShaderModule vs = wgpu_device_create_shader_module(device, ref shaderModuleDesc);

        shaderModuleDesc.code = fragmentShader;
        WGpuShaderModule fs = wgpu_device_create_shader_module(device, ref shaderModuleDesc);

        WGpuRenderPipelineDescriptor renderPipelineDesc = GetWGPU_RENDER_PIPELINE_DESCRIPTOR_DEFAULT_INITIALIZER();
        renderPipelineDesc.vertex.module = vs;
        renderPipelineDesc.vertex.entryPoint = "main";
        renderPipelineDesc.fragment.module = fs;
        renderPipelineDesc.fragment.entryPoint = "main";

        WGpuColorTargetState colorTarget = GetWGPU_COLOR_TARGET_STATE_DEFAULT_INITIALIZER();
        colorTarget.format = config.format;
        renderPipelineDesc.fragment.numTargets = 1;
        renderPipelineDesc.fragment.targets = &colorTarget;

        renderPipeline = wgpu_device_create_render_pipeline(device, ref renderPipelineDesc);

        emscripten_request_animation_frame_loop(&raf, IntPtr.Zero);
    }

    [UnmanagedCallersOnly]
    static unsafe int raf(double time, void* userData)
    {
        WGpuCommandEncoder encoder = Interop.wgpu_device_create_command_encoder_simple(device);

        WGpuRenderPassColorAttachment colorAttachment = GetWGPU_RENDER_PASS_COLOR_ATTACHMENT_DEFAULT_INITIALIZER();
        colorAttachment.view = wgpu_texture_create_view(wgpu_canvas_context_get_current_texture(canvasContext), (WGpuTextureViewDescriptor*)IntPtr.Zero);

        WGpuRenderPassDescriptor passDesc = default;
        passDesc.numColorAttachments = 1;
        passDesc.colorAttachments = &colorAttachment;

        WGpuRenderPassEncoder pass = wgpu_command_encoder_begin_render_pass(encoder, ref passDesc);
        wgpu_render_pass_encoder_set_pipeline(pass, renderPipeline);
        wgpu_render_pass_encoder_draw(pass, 3, 1, 0, 0);
        wgpu_render_pass_encoder_end(pass);

        WGpuCommandBuffer commandBuffer = wgpu_command_encoder_finish(encoder);

        wgpu_queue_submit_one_and_destroy(queue, commandBuffer);

        return 0; // Render just one frame, static content
    }
    */
}