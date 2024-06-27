using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WebGpu;
using static WebGpu.Interop;

public static class Program
{
    private static IntPtr adapter;
    private static IntPtr canvasContext;
    private static IntPtr device;
    private static IntPtr queue;
    private static IntPtr renderPipeline;

    static void Main(string[] args)
    {
        StartCalls();
    }

    private static unsafe void StartCalls()
    {
        WGpuRequestAdapterOptions options = default;
        options.powerPreference = WGPU_POWER_PREFERENCE_LOW_POWER;
        navigator_gpu_request_adapter_async(ref options, new WGpuRequestAdapterCallbackWrapper(){ cb = ObtainedWebGpuAdapter }, IntPtr.Zero);
    }

    [MonoPInvokeCallback(typeof(WGpuRequestAdapterCallback))]
    static unsafe void ObtainedWebGpuAdapter(IntPtr result, IntPtr userData)
    {
        adapter = result;

        if (adapter == IntPtr.Zero)
            Console.WriteLine("ObtainedWebGpuAdapter was given a null GpuAdapter :-(");

        WGpuDeviceDescriptor deviceDesc = default;
        wgpu_adapter_request_device_async(adapter, ref deviceDesc, new WGpuRequestDeviceCallbackWrapper() { cb = ObtainedWebGpuDevice }, IntPtr.Zero);
    }

    [MonoPInvokeCallback(typeof(WGpuRequestDeviceCallback))]
    static unsafe void ObtainedWebGpuDevice(IntPtr result, IntPtr userData)
    {
        if (result == IntPtr.Zero)
            Console.WriteLine("ObtainedWebGpuDevice was given device " + result);

        device = result;
        queue = wgpu_device_get_queue(device);
        canvasContext = wgpu_canvas_get_webgpu_context("canvas");

        WGpuCanvasConfiguration config = new WGpuCanvasConfiguration();
        GetWGPU_CANVAS_CONFIGURATION_DEFAULT_INITIALIZER(ref config);

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
        IntPtr vs = wgpu_device_create_shader_module(device, ref shaderModuleDesc);

        shaderModuleDesc.code = fragmentShader;
        IntPtr fs = wgpu_device_create_shader_module(device, ref shaderModuleDesc);
        WGpuRenderPipelineDescriptor renderPipelineDesc = new WGpuRenderPipelineDescriptor();
        GetWGPU_RENDER_PIPELINE_DESCRIPTOR_DEFAULT_INITIALIZER(ref renderPipelineDesc);

        renderPipelineDesc.vertex.module = vs;
        renderPipelineDesc.vertex.entryPoint = "main";
        renderPipelineDesc.fragment.module = fs;
        renderPipelineDesc.fragment.entryPoint = "main";

        WGpuColorTargetState colorTarget = new WGpuColorTargetState();
        GetWGPU_COLOR_TARGET_STATE_DEFAULT_INITIALIZER(ref colorTarget);

        colorTarget.format = config.format;
        renderPipelineDesc.fragment.numTargets = 1;
        renderPipelineDesc.fragment.targets = &colorTarget;

        renderPipeline = wgpu_device_create_render_pipeline(device, ref renderPipelineDesc);

        emscripten_request_animation_frame_loop(new EmscriptenAnimationLoopCallbackWrapper() { cb = raf }, IntPtr.Zero);
    }

    [MonoPInvokeCallback(typeof(WGpuRequestDeviceCallback))]
    static unsafe int raf(double time, IntPtr userData)
    {
        IntPtr encoder = Interop.wgpu_device_create_command_encoder_simple(device);
        WGpuRenderPassColorAttachment colorAttachment = new WGpuRenderPassColorAttachment();
        GetWGPU_RENDER_PASS_COLOR_ATTACHMENT_DEFAULT_INITIALIZER(ref colorAttachment);
        colorAttachment.view = wgpu_texture_create_view(wgpu_canvas_context_get_current_texture(canvasContext), (WGpuTextureViewDescriptor*)IntPtr.Zero);
        WGpuRenderPassDescriptor passDesc = default;
        passDesc.numColorAttachments = 1;
        passDesc.colorAttachments = &colorAttachment;

        IntPtr pass = wgpu_command_encoder_begin_render_pass(encoder, ref passDesc);
        wgpu_render_pass_encoder_set_pipeline(pass, renderPipeline);
        wgpu_render_pass_encoder_draw(pass, 3, 1, 0, 0);
        wgpu_render_pass_encoder_end(pass);

        IntPtr commandBuffer = wgpu_command_encoder_finish(encoder);

        wgpu_queue_submit_one_and_destroy(queue, commandBuffer);

        return 0;
    } 
}